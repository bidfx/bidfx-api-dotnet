#!/usr/bin/env bash

# Check if git still has changes
if ! git diff-index --quiet HEAD --; then
    printf "Git has uncommited changes.\n"
    exit 1
fi

# Get new version numbers
CSPROJ_FILE="../BidFX.Public.API/BidFX.Public.API.csproj"
BAK="${CSPROJ_FILE}.bak"

# Check if csproj file exists
if [[ ! -f ${CSPROJ_FILE} ]]; then
    printf "Could not find csproj file: ${CSPROJ_FILE}\n"
    exit 1
fi

cp ${CSPROJ_FILE} ${BAK}

assembly_regex='<AssemblyVersion>([[:digit:]](.[[:digit:]]){2,3})</AssemblyVersion>'
contents=`cat ${CSPROJ_FILE}`
if  [[ ${contents} =~ $assembly_regex ]]
then
    printf "Current version: ${BASH_REMATCH[1]}\n"
    current_version="${BASH_REMATCH[1]}"
else
    printf "Could not obtain AssemblyVersion from csproj file\n"
    exit 1
fi

printf "What is the release version? (${current_version}): "
read user_input
if [[ -n ${user_input} ]]
then
    release_version=${user_input}
else
    release_version=${current_version}
fi
# printf "Release version will be ${release_version}\n"

lastdigit_regex='(.*\.)([[:digit:]]+)'
if [[ ${release_version} =~ $lastdigit_regex ]]
then
    new_snapshot_version="${BASH_REMATCH[1]}$((${BASH_REMATCH[2]} + 1))"
else
    printf "Could not get digits for new Snapshot version\n"
    exit 1
fi

printf "What is the new snapshot version? (${new_snapshot_version}):"
read user_input
if [[ -n ${user_input} ]]
then
    new_snapshot_version=${user_input}
fi
# printf "New snapshot version will be ${new_snapshot_version}\n"

# Update csproj with Release Version
if [[ "${current_version}" != "${release_version}" ]]
then
    # Use sed to replace versions while preserving formatting
    sed -i.tmp "s|<AssemblyVersion>.*</AssemblyVersion>|<AssemblyVersion>${release_version}</AssemblyVersion>|g" ${CSPROJ_FILE}
    sed -i.tmp "s|<FileVersion>.*</FileVersion>|<FileVersion>${release_version}</FileVersion>|g" ${CSPROJ_FILE}
    rm ${CSPROJ_FILE}.tmp
    
    # Commit to Git
    git commit -m "prepare release ${release_version}" ${CSPROJ_FILE}
    if [[ $? -ne 0 ]];
    then
        printf "Error committing, exit code ${?}\n"
        exit 1
    fi
fi

git tag "public-api-dotnet-${release_version}"
if [[ $? -ne 0 ]];
then
    printf "Error tagging, exit code ${?}\n"
    exit 1
fi

# Update to snapshot version
# Use sed to replace versions while preserving formatting
sed -i.tmp "s|<AssemblyVersion>.*</AssemblyVersion>|<AssemblyVersion>${new_snapshot_version}</AssemblyVersion>|g" ${CSPROJ_FILE}
sed -i.tmp "s|<FileVersion>.*</FileVersion>|<FileVersion>${new_snapshot_version}</FileVersion>|g" ${CSPROJ_FILE}
rm ${CSPROJ_FILE}.tmp

# Commit to git and push
git commit -m "prepare for next development iteration" ${CSPROJ_FILE}
if [[ $? -ne 0 ]];
then
    printf "Error committing next iteration, exit code ${?}\n"
    exit 1
fi
git push && git push --tags
if [[ $? -ne 0 ]];
then
    printf "Error pushing, exit code ${?}\n"
    exit 1
fi

rm ${BAK}

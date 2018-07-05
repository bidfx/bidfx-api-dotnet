#!/usr/bin/env bash

# Check if git still has changes
if ! git diff-index --quiet HEAD --; then
    printf "Git has uncommited changes.\n"
    exit 1
fi

# Get new version numbers
ASSEMBLY_FILE="../BidFX.Public.API/Properties/AssemblyInfo.cs"
BAK="${ASSEMBLY_FILE}.bak"
TEMP_FILE="${ASSEMBLY_FILE}.tmp"

cp ${ASSEMBLY_FILE} ${BAK}

assembly_regex='\[assembly: AssemblyVersion\("([[:digit:]](.[[:digit:]]){2,3})"\)\]'
contents=`cat ${ASSEMBLY_FILE}`
if  [[ ${contents} =~ $assembly_regex ]]
then
    printf "Current version: ${BASH_REMATCH[1]}\n"
    current_version="${BASH_REMATCH[1]}"
else
    printf "Could not obtain AssemblyVersion\n"
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

# Update AssemblyInfo with Release Version
assembly_file_regex='\[assembly: AssemblyFileVersion\(".*"\)\]'
if [[ "${current_version}" != "${release_version}" ]]
then
    > ${TEMP_FILE}
    cat ${ASSEMBLY_FILE} | while read line; do
        if [[ ${line} =~ $assembly_regex ]]
        then
            echo "[assembly: AssemblyVersion(\"${release_version}\")]" >> ${TEMP_FILE}
        elif [[ ${line} =~ $assembly_file_regex ]]
        then
            echo "[assembly: AssemblyFileVersion(\"${release_version}\")]" >> ${TEMP_FILE}
        else
            echo ${line} >> ${TEMP_FILE}
        fi
    done
    mv ${TEMP_FILE} ${ASSEMBLY_FILE}
    
    # Commit to Git
    
    git commit -m "prepare release ${release_version}" ${ASSEMBLY_FILE}
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
> ${TEMP_FILE}
cat ${ASSEMBLY_FILE} | while read line; do
    if [[ ${line} =~ $assembly_regex ]]
    then
        echo "[assembly: AssemblyVersion(\"${new_snapshot_version}\")]" >> ${TEMP_FILE}
    elif [[ ${line} =~ $assembly_file_regex ]]
    then
        echo "[assembly: AssemblyFileVersion(\"${new_snapshot_version}\")]" >> ${TEMP_FILE}
    else
        echo ${line} >> ${TEMP_FILE}
    fi
done
mv ${TEMP_FILE} ${ASSEMBLY_FILE}

# Commit to git and push
git commit -m "prepare for next development iteration" ${ASSEMBLY_FILE}
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
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

assembly_regex='\[assembly: AssemblyVersion\("([[:digit:]](.[[:digit:]]){2,}+(-SNAPSHOT)?)"\)\]'
contents=`cat ${ASSEMBLY_FILE}`
if  [[ ${contents} =~ $assembly_regex ]]
then
    printf "Current version: ${BASH_REMATCH[1]}\n"
    current_version="${BASH_REMATCH[1]}"
else
    printf "Could not obtain AssemblyVersion"
    exit 1
fi

snapshot_regex='(.*)-SNAPSHOT$'
if [[ ! ${current_version} =~ $snapshot_regex ]]
then
    printf "Current version is not a SNAPSHOT\n"
    exit 1
else
    release_version="${BASH_REMATCH[1]}"
fi

printf "What is the release version? (${release_version}): "
read user_input
if [[ -n ${user_input} ]]
then
    release_version=${user_input}
fi
# printf "Release version will be ${release_version}\n"

lastdigit_regex='(.*\.)([[:digit:]])'
if [[ ${release_version} =~ $lastdigit_regex ]]
then
    new_snapshot_version="${BASH_REMATCH[1]}$((${BASH_REMATCH[2]} + 1))-SNAPSHOT"
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
> ${TEMP_FILE}
assembly_file_regex='\[assembly: AssemblyFileVersion\(".*"\)\]'
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
git commit -am "prepare release ${release_version}"
if [[ $? -ne 0 ]];
then
    exit 1
fi

git tag "public-api-dotnet-${release_version}"
if [[ $? -ne 0 ]];
then
    exit 1
fi

read user_input

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
git commit -am "prepare for next development iteration"
if [[ $? -ne 0 ]];
then
    exit 1
fi
git push && git push --tags
if [[ $? -ne 0 ]];
then
    exit 1
fi

rm ${BAK}
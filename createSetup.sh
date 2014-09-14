#! /bin/sh
# Packs the game executables, content, and some additional files into a 7zip archive.

# Name of the archive
target="Squircle.7z"
# Directory containing the binaries and compiled content.
binDir="source/Squircle/Squircle/bin/x86/Release/"
# grep -E patterns to ignore files from the $binDir
ignorePattern='.*\.pdb|.*\.application|.*\.manifest|XnaExtensions\.xml'
# Additional files to include in the archive that are not located in the $binDir
additionalFiles='README.md art/docu/readme.pdf'

function must_have_7zip_installed()
{
	echo "Failed: You must have 7-zip installed and in your path."
	exit 1
}

# Check that 7z is installed and available on the commandline
which 7z &> /dev/null || must_have_7zip_installed

# Go into the $binDir
cd "$binDir" &> /dev/null

files=$(find -type f | grep -Ev "$ignorePattern" | sed 's:\./::g')

# Create the archive from
7z a -mmt -mx9 "$target" $files

# Go to the working directory again.
cd - &> /dev/null

# Move the archive from the $binDir to the current working directory.
mv "$binDir$target" "$target"

# Now add additional files to the existing archive
7z a -mmt -mx9 "$target" $additionalFiles

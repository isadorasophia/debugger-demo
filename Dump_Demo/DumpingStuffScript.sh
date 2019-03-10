#!/bin/bash
# Script with the different steps that will be used in the Dumping Stuff demo.

# 1. Compile foo.c without the -g flag. Dwarfdump should return an "empty" output

echo "Running: gcc foo.c -o foo.o"

gcc foo.c -o foo.o

echo "Running: dwarfdump foo.o"

dwarfdump foo.o


# 2. Compile foo.c with the -g flag. Dwarfdump should show all debug information
echo "Running: gcc -g foo.c -o foo.o"

gcc -g foo.c -o foo.o

echo "Running: dwarfdump foo.o"

dwarfdump foo.o

dwarfdump foo.o > dwarfdump.txt


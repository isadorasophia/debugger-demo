#include <stdio.h>

int add(int a, int b)
{
    return a + b;
}

int main(int argc, const char** argv)
{
    int x = 1;
    int y = 5;
    int z = x + y;
    char str[] = "Hello World! The result is:";

    printf("%s %d\n\n", str, add(x, z));

    return 0;
}

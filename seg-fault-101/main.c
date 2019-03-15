#include <stdio.h>
#include <stdlib.h>

void modulo(int n, int op, /* out */ int** pBar)
{
    **pBar = n % op;
}

int main()
{
    int *pBar = 0;

    modulo(7, 2, &pBar);

    printf("Modulo is %d!\n", *pBar);
    return 0;
}

#include <stdio.h>
#include <stdlib.h>

#include "helpers.h"

int main()
{
    int* p = NULL;

    int arr[] = { 1, 1, 2, 3, 5, 8 };
    int n = length(arr);

    for (int i = 0; i < n; ++i)
    {
        arr[i] *= arr[i];
    }

    for (int i = 0; i < n; i = (i + 1) % n)
    {
        int pick = arr[i];
        printf("My favorite number should be... %d!\n", pick);
    }

    return 0;
}

#include <stdio.h>

int fib(int n)
{
    switch (n) 
    {
        case 0: return 1;
        case 1: return 1;
        default: return (fib(n-2) + fib(n-1)); 
    }
}

int subfib(int n)
{
    return fib(n);
}

int main()
{
    int fibNumber = fib(3);
    int otherFibNumber = subfib(7);

    return 0;
}

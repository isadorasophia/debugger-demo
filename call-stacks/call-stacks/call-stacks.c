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

int main()
{
    int fibNumber = fib(15);

    return 0;
}

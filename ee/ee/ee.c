#include <stdio.h>
#include <stdlib.h>

#include "helpers.h"

typedef struct _Candy
{
    char* Name;
    int Quantity;
    bool Eaten;
} Candy;

typedef struct _Blob
{
    int Age;
    bool IsHappy;
    Candy Candy;
} Blob;

int main()
{
    char* strCandy = "Brigadeiro";

    Blob blob = { 21, false };
    if (blob.IsHappy)
    {
        printf("Awesome, we seem to be happy...\n");
    }
    else
    {
        printf("Terrible, terrible fate for blob.\n");
    }

    if (!blob.Candy.Eaten)
    {
        printf("I have a candy! And it's awesome! It's a %s!\n", blob.Candy.Name);
        blob.IsHappy = true;
    }
    else
    {
        printf("I wish I had a candy...\n");
    }
}

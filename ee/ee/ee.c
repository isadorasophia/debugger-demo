#include <stdio.h>
#include <stdlib.h>

#include "helpers.h"

typedef struct _Candy
{
    char* Name;

    bool Eaten;
    int Total;
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

    Blob blob = { 21, false, 0 };
    if (blob.IsHappy)
    {
        printf("Awesome, we seem to be happy...\n");
    }
    else
    {
        printf("Terrible, terrible fate for blob.\n");

        // give some candies
        blob.Candy.Total++;
    }

    if (blob.Candy.Total && !blob.Candy.Eaten)
    {
        printf("I have a candy! And it's awesome! It's a %s!\n", blob.Candy.Name); // null value + showcase pointer
        blob.IsHappy = true;
        blob.Candy.Eaten = true; // time travel...!

        blob.Candy.Total--;
    }
    else
    {
        printf("I wish I had a candy...\n");
    }
}

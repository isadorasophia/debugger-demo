#include <stdio.h>
#include "helpers.h"

typedef enum _Day
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thrusday,
    Friday,
    Saturday
} Day;

void DoVeryImportantStuff()
{
    printf("I'm doing incredibly important stuff right now.\n");

    int coffee = 15;
    while (coffee)
    {
        printf("Drinking some coffee...\n");

        coffee--;
    }
}

bool IsWeekend(Day day)
{
    switch (day)
    {
        case Sunday:
        case Saturday:
            return true;
    }

    return false;
}

int main()
{
    Day today = Friday;

    if (!IsWeekend(today))
    {
        DoVeryImportantStuff();
    }

    return 0;
}

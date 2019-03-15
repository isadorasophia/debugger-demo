#include <stdio.h>
#include "helpers.h"

int main()
{
    Day today = Monday;                 // step over
    Agenda agenda = { today, 20 };      // step over

    if (!IsWeekend(today))              // step in
    {
        DoVeryImportantStuff(&agenda);   // step in
    }

    CleanUp();

    return 0;
}

bool IsWeekend(Day day)
{
    bool result = false;    // step in => fail
    switch (day)            // step out
    {
    case Sunday:
    case Saturday:
        return true;
    }

    return false;
}

void DoVeryImportantStuff(Agenda* agenda)
{
    printf("I'm doing incredibly important stuff right now.\n"); // step instruction!

    if (agenda->Day == Monday)                                    // step over
    {
        while (agenda->Coffee--)
        {
            printf("Drinking some coffee...\n");
        }
    }

    return;                                                      // set next instruction => line 27
}

int CleanUp()
{
    printf("Cleaning, cleaning...\n"); // step into...

    return 0;                          // step in
}

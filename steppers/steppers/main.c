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

typedef struct _Agenda
{
    Day Day;
    int Coffee;
} Agenda;

void DoVeryImportantStuff(Agenda agenda)
{
    printf("I'm doing incredibly important stuff right now.\n");

    while (agenda.Coffee)
    {
        printf("Drinking some coffee...\n");

        agenda.Coffee--;
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
    Agenda agenda = { today, 20 };

    if (!IsWeekend(today))
    {
        DoVeryImportantStuff(agenda);
    }

    return 0;
}

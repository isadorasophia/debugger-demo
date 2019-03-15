#define bool    char
#define true    1
#define false   0

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

bool IsWeekend(Day day);
void DoVeryImportantStuff(Agenda* agenda);
int CleanUp();

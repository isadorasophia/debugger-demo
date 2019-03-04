#include "stdio.h"

void do_stuff(int arr[], int sz)
{
	for (int i = 0; i < sz; ++i)
	{
		arr[i] = i * (i + 1);
	}
}

int main()
{
	int random = 42;
	int arr[] = { 5, 10, 15 };

	int ab = 3;

	do_stuff(arr, sizeof(arr));

	printf("My lucky number is %d!\n", random);

	return 0;
}

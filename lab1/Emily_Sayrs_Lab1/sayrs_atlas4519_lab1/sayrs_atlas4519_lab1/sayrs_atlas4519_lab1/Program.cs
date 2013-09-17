#define IF
#define ELSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ATLS_4519_Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            int caseSwitch = 1;
            Console.WriteLine("Hello, C#!!");
            Console.WriteLine("Hello Again");
            int x = 5;
            Console.WriteLine("Bon Jour!!");
            int y = 4;
            Console.WriteLine("Guten Tag!!");
            Console.WriteLine("x = {0}; y = {1}", x, y);
            x = y;
            Console.WriteLine("x = {0}; y = {1}", x, y);

//msnd multiplication example modified 
               Console.WriteLine(x * 2);
               Console.WriteLine("y*3={0}", y * 3);//use bracets to display the value, since only one, {0} will display the value of y*3
               Console.WriteLine("y*3={1}", y * 3, y * 6);
        Console.WriteLine(x * y);


        x = (x * y);//set x to x times y
        Console.WriteLine("x = {0}; y = {1}", x, y);// display 2 ints x,y in the order 0,1 


//msnd #if directive 
            #if (IF && !ELSE)
        Console.WriteLine("IF is defined");
#elif (!IF && ELSE)
        Console.WriteLine("ELSE is defined");
#elif (IF && ELSE)
        Console.WriteLine("IF and ELSE are defined");
#else
        Console.WriteLine("IF and ELSE are not defined");
#endif


//if/else statements  
       
        if (x > y)
        {
            Console.WriteLine("{0} is greater than {1}", x,y);
        }
        else
        {
            Console.WriteLine("x is not greater than y");
        }
        if (x > 10)
        {
            Console.WriteLine("x is greater than 10");
        }
        else
        {
            Console.WriteLine("x is not greater than y");
        }

//msnd if/else example modified and working with switch statements 
        Console.Write("Enter a character: ");
        char c = (char)Console.Read();
        if (Char.IsLetter(c))
        {
            if (Char.IsLower(c))
            {
                Console.WriteLine("The character is lowercase.");
                 caseSwitch = 1;//if lowercase call case switch 1
            }
            else
            {
                Console.WriteLine("The character is uppercase.");
                 caseSwitch = 2;//if uppercase call case switch 2 
            }
        }
        else
        {
            Console.WriteLine("The character isn't an alphabetic character.");
        }


// msnd switch example modified 

        
        switch (caseSwitch)
        {
            case 1:
                Console.WriteLine("Case 1");
                // while example 
                while (y <= x)
                {
                    Console.WriteLine("x is now {0}, y is now {1}", x, y);// if lowercase letter is typed, case 1 is called, and y value increases to meet x value 
                    y++;
                }
                break;
            case 2:
                Console.WriteLine("Case 2");
                // while example 
                while (y <= x)
                {
                    Console.WriteLine("x is now {0}, y is now {1}", x, y);// if uppercase letter is typed, case 2 is called, and x value decreases to meet y value 
                    x--;
                }
                break;
            default:
                Console.WriteLine("Dafault Case");
                break;
        }
//for loop example 
        for (int i = 1; i <20; i++)
        {
            Console.WriteLine("This is a for loop", x + i, y);
        }


        int a = 0;
        for (int b = 0; b < 10; b += 2)
        {
           // if (b == 2)
               // continue;
            a++;
        }

        }
    
    }
}

/*
 * sdlfhksjdf
 * lsdjflsdkjfs
 * jlskdfjlsdkj
 **/

// dfsdf

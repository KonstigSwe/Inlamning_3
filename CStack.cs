using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlamning_3_ra_kod
{
    /* CLASS: CStack
     * PURPOSE: Is essentially a RPN-calculator with four registers X, Y, Z, T
     *   like the HP RPN calculators. Numeric values are entered in the entry
     *   string by adding digits and one comma. For test purposes the method
     *   RollSetX can be used instead. Operations can be performed on the
     *   calculator preferrably by using one of the methods
     *     1. BinOp - merges X and Y into X via an operation and rolls down
     *        the stack
     *     2. Unop - operates X and puts the result in X with overwrite
     *     3. Nilop - adds a known constant on the stack and rolls up the stack
     */
    public class CStack
    {
        public double X, Y, Z, T;
        public string entry, LetterChosen;
        public string[,] storeVar = new string[8, 2];
        /* CONSTRUCTOR: CStack
         * PURPOSE: create a new stack and init X, Y, Z, T and the text entry, Assigns storeVar its letters And loads a file
         * PARAMETERS: --
         */
        public CStack()
        {
            X = Y = Z = T = 0;
            entry = "";
            storeVar[0, 0] = "A";
            storeVar[1, 0] = "B";
            storeVar[2, 0] = "C";
            storeVar[3, 0] = "D";
            storeVar[4, 0] = "E";
            storeVar[5, 0] = "F";
            storeVar[6, 0] = "G";
            storeVar[7, 0] = "H";

            try
            {
                LoadFile();
            }
            catch { }
        }
        /* METHOD: Exit
         * PURPOSE: called on exit, prepared for saving
         * PARAMETERS: --
         * RETURNS: --
         */
        public void Exit()
        {
            FileSave();
        }
        /* METHOD: StackString
         * PURPOSE: construct a string to write out in a stack view
         * PARAMETERS: --
         * RETURNS: the string containing the values T, Z, Y, X with newlines 
         *   between them
         */
        public string StackString()
        {
            return $"{T}\n{Z}\n{Y}\n{X}\n{entry}";
        }
        /* METHOD: VarString
         * PURPOSE: construct a string to write out in a variable list
         * PARAMETERS: tempStor = stores the numbers in storVar
         * RETURNS: tempStor
         */
        public string VarString()
        {
            string[] tempStor = new string[8];
            for (int i = 0; i < tempStor.Length; i++)
            {
                if (tempStor[i] == null)
                {
                    tempStor[i] = "0";
                }
                try
                {
                    tempStor[i] = storeVar[i, 1];
                }
                catch
                {

                }


            }
            return $"{tempStor[0]}\n{tempStor[1]}\n{tempStor[2]}\n{tempStor[3]}\n{tempStor[4]}\n{tempStor[5]}\n{tempStor[6]}\n{tempStor[7]}\n";
        }
        /* METHOD: SetX
         * PURPOSE: set X with overwrite
         * PARAMETERS: double newX - the new value to put in X
         * RETURNS: --
         */
        public void SetX(double newX)
        {
            X = newX;
        }
        /* METHOD: EntryAddNum
         * PURPOSE: add a digit to the entry string
         * PARAMETERS: string digit - the candidate digit to add at the end of the
         *   string
         * RETURNS: --
         * FAILS: if the string digit does not contain a parseable integer, nothing
         *   is added to the entry
         */
        public void EntryAddNum(string digit)
        {
            int val;
            if (int.TryParse(digit, out val))
            {
                entry = entry + val;
            }
        }
        /* METHOD: EntryAddComma
         * PURPOSE: adds a comma to the entry string
         * PARAMETERS: --
         * RETURNS: --
         * FAILS: if the entry string already contains a comma, nothing is added
         *   to the entry
         */
        public void EntryAddComma()
        {
            if (entry.IndexOf(",") == -1)
                entry = entry + ",";
        }
        /* METHOD: EntryChangeSign
         * PURPOSE: changes the sign of the entry string
         * PARAMETERS: --
         * RETURNS: --
         * FEATURES: if the first char is already a '-' it is exchanged for a '+',
         *   if it is a '+' it is changed to a '-', otherwise a '-' is just added
         *   first
         */
        public void EntryChangeSign()
        {
            char[] cval = entry.ToCharArray();
            if (cval.Length > 0)
            {
                switch (cval[0])
                {
                    case '+': cval[0] = '-'; entry = new string(cval); break;
                    case '-': cval[0] = '+'; entry = new string(cval); break;
                    default: entry = '-' + entry; break;
                }
            }
            else
            {
                entry = '-' + entry;
            }
        }
        /* METHOD: Enter
         * PURPOSE: converts the entry to a double and puts it into X
         * PARAMETERS: --
         * RETURNS: --
         * FEATURES: the entry is cleared after a successful operation
         */
        public void Enter()
        {
            if (entry != "")
            {
                RollSetX(double.Parse(entry));
                entry = "";
            }
        }
        /* METHOD: Drop
         * PURPOSE: drops the value of X, and rolls down
         * PARAMETERS: --
         * RETURNS: --
         * FEATURES: Z gets the value of T
         */
        public void Drop()
        {
            X = Y; Y = Z; Z = T;
        }
        /* METHOD: DropSetX
         * PURPOSE: replaces the value of X, and rolls down
         * PARAMETERS: double newX - the new value to assign to X
         * RETURNS: --
         * FEATURES: Z gets the value of T
         * NOTES: this is used when applying binary operations consuming
         *   X and Y and putting the result in X, while rolling down the
         *   stack
         */
        public void DropSetX(double newX)
        {
            X = newX; Y = Z; Z = T;
        }
        /* METHOD: BinOp
         * PURPOSE: evaluates a binary operation
         * PARAMETERS: string op - the binary operation retrieved from the
         *   GUI buttons
         * RETURNS: --
         * FEATURES: the stack is rolled down
         */
        public void BinOp(string op)
        {
            switch (op)
            {
                case "+": DropSetX(Y + X); break;
                case "−": DropSetX(Y - X); break;
                case "×": DropSetX(Y * X); break;
                case "÷": DropSetX(Y / X); break;
                case "yˣ": DropSetX(Math.Pow(Y, X)); break;
                case "ˣ√y": DropSetX(Math.Pow(Y, 1.0 / X)); break;
            }
        }
        /* METHOD: Unop
         * PURPOSE: evaluates a unary operation
         * PARAMETERS: string op - the unary operation retrieved from the
         *   GUI buttons
         * RETURNS: --
         * FEATURES: the stack is not moved, X is replaced by the result of
         *   the operation
         */
        public void Unop(string op)
        {
            switch (op)
            {
                // Powers & Logarithms:
                case "x²": SetX(X * X); break;
                case "√x": SetX(Math.Sqrt(X)); break;
                case "log x": SetX(Math.Log10(X)); break;
                case "ln x": SetX(Math.Log(X)); break;
                case "10ˣ": SetX(Math.Pow(10, X)); break;
                case "eˣ": SetX(Math.Exp(X)); break;

                // Trigonometry:
                case "sin": SetX(Math.Sin(X)); break;
                case "cos": SetX(Math.Cos(X)); break;
                case "tan": SetX(Math.Tan(X)); break;
                case "sin⁻¹": SetX(Math.Asin(X)); break;
                case "cos⁻¹": SetX(Math.Acos(X)); break;
                case "tan⁻¹": SetX(Math.Atan(X)); break;
            }
        }
        /* METHOD: Nilop
         * PURPOSE: evaluates a "nilary operation" (insertion of a constant)
         * PARAMETERS: string op - the nilary operation (name of the constant)
         *   retrieved from the GUI buttons
         * RETURNS: --
         * FEATURES: the stack is rolled up, X is preserved in Y that is preserved in
         *   Z that is preserved in T, T is erased
         */
        public void Nilop(string op)
        {
            switch (op)
            {
                case "π": RollSetX(Math.PI); break;
                case "e": RollSetX(Math.E); break;
            }
        }
        /* METHOD: Roll
         * PURPOSE: rolls the stack up
         * PARAMETERS: --
         * RETURNS: --
         */
        public void Roll()
        {
            double tmp = T;
            T = Z; Z = Y; Y = X; X = tmp;
        }
        /* METHOD: Roll
         * PURPOSE: rolls the stack up and puts a new value in X
         * PARAMETERS: double newX - the new value to put into X
         * RETURNS: --
         * FEATURES: T is dropped
         */
        public void RollSetX(double newX)
        {
            T = Z; Z = Y; Y = X; X = newX;
        }
        /* METHOD: SetAddress
         * PURPOSE: To save the latest pushed button of A-H to save things to its possition in SetVar and Get Var
         * PARAMETERS: string name - variable name, LetterChosen - the choosen letter
         * RETURNS: --
         */
        public void SetAddress(string name)
        {
            LetterChosen = name;
        }
        /* METHOD: SetVar
         * PURPOSE: to give a number in the list storeVar its accompanied letter of A-H
         * PARAMETERS:temp = temporary string save of what X is., 
         * RETURNS: --
         * FEATURES: assigns an number to the choosen letter in the list
         */
        public void SetVar()
        {


            string temp = X.ToString();
            for (int i = 0; i < 8; i++)
            {
                if (LetterChosen == storeVar[i, 0])
                {
                    storeVar[i, 1] = temp;
                }
            }
        }
        /* METHOD: GetVar
         * PURPOSE: to get a variable from the stored list storeVar with A-H as variable saves
         * PARAMETERS:newX = new X number
         * RETURNS: --
         *  FEATURES: takes the number assigned to the letter in the list
         */
        public void GetVar()
        {
            double newX = 0;
            for (int i = 0; i < 8; i++)
            {
                if (LetterChosen == storeVar[i, 0])
                {

                    newX = double.Parse(storeVar[i, 1]);
                }
            }
            RollSetX(newX);
        }
        /* METHOD: LoadFile
        * PURPOSE: Load X,Y,Z,T from a file
        * PARAMETERS: path = as the name says
        * RETURNS:--
        */
        public void LoadFile()// Load the specified file
        {
            string path = @"C:\Users\nikla\Documents\Visual_studio_code\inlamning_3_ra_kod\AssignedNumbers.txt";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    string[] words = line.Split('=');
                    switch (words[0])
                    {
                        case "X":
                            X = double.Parse(words[1]);
                            break;
                        case "Y":
                            Y = double.Parse(words[1]);
                            break;
                        case "Z":
                            Z = double.Parse(words[1]);
                            break;
                        case "T":
                            T = double.Parse(words[1]);
                            break;
                        case "A":
                            storeVar[0, 1] = words[1];
                            break;
                        case "B":
                            storeVar[1, 1] = words[1];
                            break;
                        case "C":
                            storeVar[2, 1] = words[1];
                            break;
                        case "D":
                            storeVar[3, 1] = words[1];
                            break;
                        case "E":
                            storeVar[4, 1] = words[1];
                            break;
                        case "F":
                            storeVar[5, 1] = words[1];
                            break;
                        case "G":
                            storeVar[6, 1] = words[1];
                            break;
                        case "H":
                            storeVar[7, 1] = words[1];
                            break;
                    }
                }
            }
            Console.WriteLine("Loaded the file");
        }
        /* METHOD: FileSave
         * PURPOSE: Save X,Y,Z,T in a file
         * PARAMETERS: Filelocation = location of the file
         * RETURNS:--
         */
        public void FileSave()// saves the list to a file
        {
            string fileLocation = @"C:\Users\nikla\Documents\Visual_studio_code\inlamning_3_ra_kod\AssignedNumbers.txt";

            if (!System.IO.File.Exists(fileLocation))
            {

                // https://docs.microsoft.com/en-us/dotnet/api/system.io.file.createtext?view=net-5.0 Creates file at specified location
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(fileLocation))
                {

                    sw.WriteLine("X={0}", X);
                    sw.WriteLine("Y={0}", Y);
                    sw.WriteLine("Z={0}", Z);
                    sw.WriteLine("T={0}", T);
                    sw.WriteLine("A={0}", storeVar[0, 1]);
                    sw.WriteLine("B={0}", storeVar[1, 1]);
                    sw.WriteLine("C={0}", storeVar[2, 1]);
                    sw.WriteLine("D={0}", storeVar[3, 1]);
                    sw.WriteLine("E={0}", storeVar[4, 1]);
                    sw.WriteLine("F={0}", storeVar[5, 1]);
                    sw.WriteLine("G={0}", storeVar[6, 1]);
                    sw.WriteLine("H={0}", storeVar[7, 1]);
                }
            }
            else
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileLocation))
                {
                    sw.WriteLine("X={0}", X);
                    sw.WriteLine("Y={0}", Y);
                    sw.WriteLine("Z={0}", Z);
                    sw.WriteLine("T={0}", T);
                    sw.WriteLine("A={0}", storeVar[0, 1]);
                    sw.WriteLine("B={0}", storeVar[1, 1]);
                    sw.WriteLine("C={0}", storeVar[2, 1]);
                    sw.WriteLine("D={0}", storeVar[3, 1]);
                    sw.WriteLine("E={0}", storeVar[4, 1]);
                    sw.WriteLine("F={0}", storeVar[5, 1]);
                    sw.WriteLine("G={0}", storeVar[6, 1]);
                    sw.WriteLine("H={0}", storeVar[7, 1]);
                }
            }
            Console.WriteLine("Saved");
        }
    }
}


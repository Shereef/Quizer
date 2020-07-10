using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quizer
{
    class Program
    {
        public class Question
        {
            readonly string question;
            readonly string answer;
            readonly List<string> fakeAnswers;
            List<string> chosenFakeAnswers;

            public Question(string question, string answer, List<string> answers)
            {
                this.question = question;
                this.answer = answer;
                this.fakeAnswers = answers.Where(a => a != answer).ToList();
            }

            public List<string> getAnswers()
            {
                if (chosenFakeAnswers == null)
                {
                    Random rnd = new Random(DateTime.Now.Millisecond);
                    chosenFakeAnswers = new List<string>();
                    for (int i = 0; i < 3; i++)
                    {
                        IEnumerable<string> filteredPossibleAnswers = fakeAnswers.Where(a => !chosenFakeAnswers.ConvertAll(a=>a.Replace('.', '').Trim().ToLowerCase()).Contains(a.ConvertAll(a=>a.Replace('.', '').Trim().ToLowerCase())));
                        string item = filteredPossibleAnswers.ToList()[rnd.Next(filteredPossibleAnswers.Count())];
                        fakeAnswers.Remove(item);
                        chosenFakeAnswers.Add(item);
                    }
                    chosenFakeAnswers.Insert(rnd.Next(chosenFakeAnswers.Count + 1), answer);
                }
                return chosenFakeAnswers;
            }

            public bool isRightAnswer(string answer) => answer == this.answer;

            public string getQuestion() => question;
        }
        static void Main(string[] args)
        {
            string[] questions = null;
            string[] answers = null;

            try
            {
                questions = File.ReadAllLines("questions.txt");
                answers = File.ReadAllLines("answers.txt");
            }
            catch (Exception)
            {
                // ignore
            }
            if (questions == null || questions.Length == 0)
            {
                Console.WriteLine("Please provide a list of questions in questions.txt file 1 question per line!");
                Console.ReadKey();
                return;
            }

            if (answers == null || answers.Length < 4)
            {
                Console.WriteLine("Please provide a list of answers in answers.txt file 1 answer per line with a minimum of 4 answers");
                Console.WriteLine("make sure the correct answer is on the same line as the question");
                Console.ReadKey();
                return;
            }
            if (questions.Length > answers.Length)
            {
                Console.WriteLine("Make sure you have more answers than questions or an equal number");
                Console.ReadKey();
                return;

            }
            List<Question> questionsList = new List<Question>(questions.Length);
            for (int i = 0; i < questions.Length; i++)
            {
                string question = questions[i];
                if (string.IsNullOrWhiteSpace(question))
                    continue;
                if (string.IsNullOrWhiteSpace(answers[i]))
                {
                    Console.WriteLine($"Answer on line {i + 1} is empty! please check it question:{question}");
                    Console.ReadKey();
                    return;
                }
                Question thisQuestion = new Question(question, answers[i], answers.ToList());
                questionsList.Add(thisQuestion);
            }
            int counter = 1;
            Console.WriteLine("Please answer the following questions:");
            while (questionsList.Any())
            {
                Random rnd = new Random(DateTime.Now.Millisecond);
                Question thisQuestion = questionsList[rnd.Next(questionsList.Count)];
                Console.WriteLine($"{counter++} - {thisQuestion.getQuestion()}?");
                Console.WriteLine();
                int answerCounter = 1;
                List<string> myAnswers = thisQuestion.getAnswers();
                foreach (string answer in myAnswers)
                {
                    Console.WriteLine($"{answerCounter++} - {answer}.");
                }
                char choice = getNumber();
                int number = isBetween1And4(ref choice);
                while (!thisQuestion.isRightAnswer(myAnswers[number - 1]))
                {

                    Console.WriteLine("Wrong! try again!");
                    choice = getNumber();
                    number = isBetween1And4(ref choice);
                }
                questionsList.Remove(thisQuestion);
                Console.WriteLine("Correct!");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine("Well done!");
            Console.ReadKey();
        }

        private static char getNumber()
        {
            char choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            while (!char.IsNumber(choice))
            {
                Console.WriteLine("Please enter a number between 1 and 4!");
                choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
            }

            return choice;
        }

        private static int isBetween1And4(ref char choice)
        {
            int number = int.Parse(choice.ToString());
            while (number < 1 || number > 4)
            {
                Console.WriteLine("Please enter a number between 1 and 4!");
                choice = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (char.IsNumber(choice))
                    number = int.Parse(choice.ToString());
            }

            return number;
        }
    }
}

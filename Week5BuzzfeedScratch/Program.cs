using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Week5BuzzfeedScratch
{
    class Program
    {
        static void Main(string[] args)
        {

            // solve for apostophes

            string makin = "";
            Quizzes quiz = new Quizzes();
            SqlConnection connection = new SqlConnection(@"Data Source=minecraft.lfgpgh.com;Initial Catalog=S13-BuzzFeed;Persist Security Info=True;User ID=academy_student;Password=12345");
            Console.WriteLine("Would you like to make a quiz? Y)es or N)o");
            connection.Open();

            while (makin != "n")
            {
                makin = Console.ReadLine().ToLower();

                if (makin == "y")
                {
                    // Get quiz's name
                    Console.WriteLine("What is the quiz's name?");
                    quiz.name = Console.ReadLine();

                    CreateQuiz(quiz.name, connection);

                    SqlCommand getQuizId = new SqlCommand($"SELECT * FROM Quizzes WHERE Name = '{quiz.name}'", connection);

                    // We need to find the quiz ID!
                    SqlDataReader reader = getQuizId.ExecuteReader();

                    quiz.id = 0;
                    while (reader.Read())
                    {
                        quiz.id = Convert.ToInt32($"{reader["Id"]}");
                    }
                    reader.Close();
                    Console.WriteLine($"Your Quiz, '{quiz.name}' is Quiz #{quiz.id}!");


                    Console.WriteLine("How many questions would you like to ask in this quiz?");
                    quiz.questionsCount = Convert.ToInt32(Console.ReadLine());

                    //how do you catch an error? try/catch block??

                    CreateResults(quiz.id, quiz.questionsCount, connection);

                    CreateQuestions(quiz.id, quiz.questionsCount, connection);

                    Console.WriteLine($"Alright! Your quiz '{quiz.name}' is set up and ready to go!");
                    Console.WriteLine("Would you like to make another quiz? Y)es or N)o");
                    makin = Console.ReadLine();

                    if (makin == "n")
                    {
                        Console.WriteLine("Ok maybe next time.");
                    }
                    else { }
                }
                else if (makin == "n")
                {
                    Console.WriteLine("Ok maybe next time.");
                }
                else
                {
                    Console.WriteLine("I'm sorry... so do you or don't you? \nMake a quiz? Y)es or N)o");
                }

            }

            connection.Close();

        }
        static void CreateQuiz(string quizName, SqlConnection connection)
        {
            
            SqlCommand command = new SqlCommand($"INSERT INTO Quizzes (Name) VALUES ('{quizName}')", connection);
            command.ExecuteNonQuery();
            
        }

        static void CreateQuestions(int quizID, int qCount, SqlConnection connection)
        {

            for (int i = 0; i < qCount; i++)
            {
                
                int qCounter = i + 1;
                Questions tempQuestion = new Questions();

                Console.WriteLine($"Enter question #{qCounter} for the quiz!");
                tempQuestion.text = Console.ReadLine().ToString();

                tempQuestion.quizId = quizID;

                SqlCommand commandQ = new SqlCommand($"INSERT INTO [Questions] (QuizId, Text) VALUES ('{quizID}', '{tempQuestion.text}')", connection);
                commandQ.ExecuteNonQuery();

                commandQ = new SqlCommand($"SELECT * FROM [Questions] WHERE Text = '{tempQuestion.text}'", connection);
                // We need to find question ID!
                SqlDataReader reader2 = commandQ.ExecuteReader();
                tempQuestion.id = 0;
                while (reader2.Read())
                {
                    tempQuestion.id = Convert.ToInt32($"{reader2["Id"]}");
                }
                reader2.Close();

                CreateAnswers(quizID, tempQuestion.id, tempQuestion.text, connection);
            }

            
        }

        static void CreateAnswers(int quizID, int questionID, string question, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand($"SELECT * FROM Results WHERE QuizId = '{quizID}'", connection);

            SqlDataReader reader = cmd.ExecuteReader();

            //loop over the results for a quiz
            //grab each result and put it into an array

            List<Results> results = new List<Results>();

            while (reader.Read())
            {
                Results tempResult = new Results();
                tempResult.title = reader["Title"].ToString();

                results.Add(tempResult);

            }
            reader.Close();

            //hardcode # of answers to 4 to make life easier
            int count = 4;
            Console.WriteLine($"Enter {count} answers for the question!");



            for (int j = 0; j < count; j++)
            {
                
                Answers tempAnswer = new Answers();
                tempAnswer.value = j;
                int ansCount = j + 1;
                //instead of value, this is where we would put the corresponding result for the answer they're creating
                

                Console.WriteLine($"answer #{ansCount} of {count} for '{question}'");
                Console.WriteLine($"This answer corresponds with the result '{results[j].title}'");

                Console.WriteLine("Enter the answer!");
                tempAnswer.text = Console.ReadLine();


                SqlCommand answerCommand = new SqlCommand($"INSERT INTO [Answers] (Text, Value, QuestionId) VALUES ('{tempAnswer.text}', '{tempAnswer.value}', '{questionID}')", connection);
                answerCommand.ExecuteNonQuery();

                Console.Clear();

            

               
            }
        }


        static void CreateResults(int quizID, int questionCount, SqlConnection connection)
        {
            //Results results = new Results();
            Console.WriteLine("What are the possible results for this quiz? (Pick 4 results)");

            
            int resultCount = 4;

            int i_but_better = 0;
            for (int i = 0; i <= (resultCount - 1); i++)
            {
                Results results = new Results();

                results.min = questionCount * i_but_better;
                results.max = questionCount * (i_but_better += 1);

                Console.WriteLine("Enter a result!");
                results.title = Console.ReadLine().ToString();

                Console.WriteLine("Now enter a description! What does this result say about the person?");
                results.description = Console.ReadLine().ToString();

                SqlCommand resultCommand = new SqlCommand($"INSERT INTO [Results] (Min, Max, Description, Title, QuizID) VALUES ('{results.min}', '{results.max}', '{results.description}', '{results.title}', '{quizID}')", connection);

                resultCommand.ExecuteNonQuery();

                Console.Clear();
                //for value range
                //Console.WriteLine("Min = " + results.min);
                //Console.WriteLine("Max = " + results.max);
            }
        }

       

    }
    public class Quizzes
    {
        
        public int id;
        public int questionsCount;
        public List<Questions> questionsList;
        public string name;

    }
    public class Questions
    {
        public int id;
        public string text;
        public int quizId;

        public List<Answers> answersList;
    }
    public class Answers
    {
        public int id;
        public string text;
        public int value;
        public int questionId;

        
    }
    public class Results
    {
        public int id;
        public int min;
        public int max;
        public string description;
        public string title;
        public int quizID;
        public List<Results> resultsList;

    }

        
    
}

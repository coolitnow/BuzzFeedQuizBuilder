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

            // solve for apostrophes

            string makinQuiz = "";

            //create new instance of quiz class 
            Quizzes quiz = new Quizzes();

            //connect to database and create variable "connection" in order to access the connection
            SqlConnection connection = new SqlConnection(@"CONNECTIONINFO");

            //Ask user if they'd like to make a quiz, thus opening while loop or skipping it
            Console.WriteLine("Would you like to make a quiz? Y)es or N)o");

            //open the connection
            connection.Open();

            while (makinQuiz != "n")
            {
                makinQuiz = Console.ReadLine().ToLower();

                if (makinQuiz == "y")
                {
                    // Get quiz's name, assign it to name aspect of Quiz class
                    Console.WriteLine("What is the quiz's name?");
                    quiz.name = Console.ReadLine();

                    //call CreateQuiz Function, using sql connection. The Quiz ID is assigned in this function 
                    CreateQuiz(quiz.name, connection);

                    //Grab assigned QuizID from the tables Quizzes
                    SqlCommand getQuizId = new SqlCommand($"SELECT * FROM Quizzes WHERE Name = '{quiz.name}'", connection);

                    // We need to find the quiz ID! Execute SQL reader
                    SqlDataReader reader = getQuizId.ExecuteReader();


                    //give quiz.id a value here so it can reset if user makes another quiz and it can holed that assigned value after the loop
                    quiz.id = 0;
                    while (reader.Read())
                    {
                        quiz.id = Convert.ToInt32($"{reader["Id"]}");
                    }
                    reader.Close();

                    
                    Console.WriteLine($"Your Quiz, '{quiz.name}' is Quiz #{quiz.id}!");

                    //set number of questions here
                    Console.WriteLine("How many questions would you like to ask in this quiz?");
                    quiz.questionsCount = Convert.ToInt32(Console.ReadLine());

                    //how do you catch an error? try/catch block??

                    //once # of q's is received, call the CreateResults function
                    CreateResults(quiz.id, quiz.questionsCount, connection);

                    //CreateQuestions function called after the Results are created
                    CreateQuestions(quiz.id, quiz.questionsCount, connection);

                    Console.WriteLine($"Alright! Your quiz '{quiz.name}' is set up and ready to go!");
                    Console.WriteLine("Would you like to make another quiz? Y)es or N)o");
                    makinQuiz = Console.ReadLine();

                    if (makinQuiz == "n")
                    {
                        Console.WriteLine("Ok maybe next time.");
                    }
                    else { }
                }
                else if (makinQuiz == "n")
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


        //this function repeats for however many questions they have chosen to create
        static void CreateQuestions(int quizID, int qCount, SqlConnection connection)
        {

            for (int i = 0; i < qCount; i++)
            {
                //a counter to show user which number they're on
                int qCounter = i + 1;

                //create new instance of Questions class - temporary and reusable in this loop
                Questions tempQuestion = new Questions();

                Console.WriteLine($"Enter question #{qCounter} for the quiz!");
                tempQuestion.text = Console.ReadLine().ToString();

                tempQuestion.quizId = quizID;

                SqlCommand commandQ = new SqlCommand($"INSERT INTO [Questions] (QuizId, Text) VALUES ('{quizID}', '{tempQuestion.text}')", connection);
                //no reader necessary since info is only being placed in table
                commandQ.ExecuteNonQuery();

                commandQ = new SqlCommand($"SELECT * FROM [Questions] WHERE Text = '{tempQuestion.text}'", connection);
                // We need to find question ID! reader necessary to retrieve info from the query

                SqlDataReader reader2 = commandQ.ExecuteReader();
                //set variable value so it will exist after the reader loop
                tempQuestion.id = 0;
                while (reader2.Read())
                {
                    tempQuestion.id = Convert.ToInt32($"{reader2["Id"]}");
                }
                reader2.Close();


                //now call CreateAnswers function to create the answers for the newly made question
                CreateAnswers(quizID, tempQuestion.id, tempQuestion.text, connection);
            }

            
        }

        //passing the question as a string helps user remember what they're creating answers for
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
            //figure out how to make this whatever they want
            int count = 4;
            Console.WriteLine($"Enter {count} answers for the question!");



            for (int j = 0; j < count; j++)
            {
                //create new and temporary instance of Answers class to create the answers for the question
                Answers tempAnswer = new Answers();
                tempAnswer.value = j;

                //instead of tempAnswer.value, ansCount is where we would put the corresponding result for the answer they're creating ie 1 of 4, 2 of 4, etc
                int ansCount = j + 1;
               
                
                Console.WriteLine($"answer #{ansCount} of {count} for '{question}'");

                //Remind user which result this answer corresponds to
                Console.WriteLine($"This answer corresponds with the result '{results[j].title}'");

                Console.WriteLine("Enter the answer!");
                tempAnswer.text = Console.ReadLine();


                SqlCommand answerCommand = new SqlCommand($"INSERT INTO [Answers] (Text, Value, QuestionId) VALUES ('{tempAnswer.text}', '{tempAnswer.value}', '{questionID}')", connection);
                answerCommand.ExecuteNonQuery();
                //ExecuteNonQuery since we are entering info, do not need any returned

                Console.Clear();

            

               
            }
        }

        //pass quiz.id, quiz.questionsCount and SQL connection
        static void CreateResults(int quizID, int questionCount, SqlConnection connection)
        {
            Console.WriteLine("What are the possible results for this quiz? (Pick 4 results)");

            //result count hardcoded to 4 to make initial math easier.
            //figure out how to let user change count
            int resultCount = 4;

            int i_but_better = 0;
            for (int i = 0; i <= (resultCount - 1); i++)
            {
                //create new instance of Results class
                Results results = new Results();

                results.min = questionCount * i_but_better;
                results.max = questionCount * (i_but_better += 1);

                Console.WriteLine("Enter a result!");
                results.title = Console.ReadLine().ToString();

                Console.WriteLine("Now enter a description! What does this result, " + results.title + " say about the person?");
                results.description = Console.ReadLine().ToString();

                //insert information into Results table. Holds Quiz ID as an identifier
                SqlCommand resultCommand = new SqlCommand($"INSERT INTO [Results] (Min, Max, Description, Title, QuizID) VALUES ('{results.min}', '{results.max}', '{results.description}', '{results.title}', '{quizID}')", connection);

                resultCommand.ExecuteNonQuery();

                Console.Clear();
                //Console.WriteLine("Min = " + results.min);
                //Console.WriteLine("Max = " + results.max);
            }

            //function ends after 4 results are entered
        }

       

    }
    public class Quizzes
    {
        
        public int id;
        public int questionsCount;
        public string name;

    }
    public class Questions
    {
        public int id;
        public string text;
        public int quizId;

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

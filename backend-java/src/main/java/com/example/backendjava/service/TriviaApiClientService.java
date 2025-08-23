package com.example.backendjava.service;

import com.example.backendjava.model.Category;
import com.example.backendjava.model.Quiz;
import com.example.backendjava.model.TriviaQuestion;
import com.example.backendjava.model.QuizQuestion;
import com.example.backendjava.repository.CategoryRepository;
import com.example.backendjava.repository.QuizRepository;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;


import java.io.InputStreamReader;
import java.lang.reflect.Type;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Optional;

@Service
public class TriviaApiClientService {

    private final String API_URL = "https://the-trivia-api.com/api/questions?limit=5";

    @Autowired
    private QuizRepository quizRepository;

    @Autowired
    private CategoryRepository categoryRepository;

    //This Method reads the fetched Qs from triviaQuestions and stores them in DB for the Controllers to read later on
    public Quiz generateAndSaveApiQuiz(String difficulty, String categoryName) {
        try {
            //Generates an empty List of Questions
            List<TriviaQuestion> triviaQuestions = fetchQuestions(difficulty, categoryName);

            //Generates a new Quiz Object
            Quiz quiz = new Quiz("API Quiz", "Trivia API has provided this Quiz");

            //Assign category if it exists to Quiz Object
            if (categoryName != null && !categoryName.isBlank()) {
                Optional<Category> categoryOpt = categoryRepository.findByNameIgnoreCase(categoryName.trim());
                if (categoryOpt.isPresent()) {
                    quiz.setCategory(categoryOpt.get());
                    System.out.println("Category set on quiz: " + categoryOpt.get().getName());
                } else {
                    System.out.println("Could not find category with name: " + categoryName);
                }
            }

            //From Fetched Questions, add all answers to the Object and shuffle
            for (TriviaQuestion tq : triviaQuestions) {
                List<String> allAnswers = new ArrayList<>(tq.getIncorrectAnswers());
                allAnswers.add(tq.getCorrectAnswer());
                Collections.shuffle(allAnswers);

                quiz.addQuestion(new QuizQuestion(
                        tq.getQuestion(),
                        allAnswers,
                        tq.getCorrectAnswer()
                ));
            }
            //Save quiz in the DB
            return quizRepository.save(quiz);
        } catch (Exception e) {
            throw new RuntimeException("Failed to fetch or save AI quiz: " + e.getMessage(), e);
        }
    }

    //Method that returns List of Questions to the Controller
    public List<TriviaQuestion> fetchRandomQuestions() throws Exception {
        return fetchQuestions(null, null);
    }

    //Method to fetch Questions from the API, parse them with GSON and save them into the TriviaQuestion List
    public List<TriviaQuestion> fetchQuestions(String difficulty, String category) throws Exception {
        String apiUrl = API_URL;

        //Adding the difficulty chosen to the API request
        if (difficulty != null && !difficulty.isBlank()) {
            apiUrl += "&difficulty=" + difficulty.toLowerCase();
        }

        // Add category chosen to the API request
        if (category != null && !category.isBlank()) {
            // The API expects dash-separated values like `arts-and-literature`
            String formattedCategory = category.toLowerCase().replace("_", "-");
            apiUrl += "&categories=" + formattedCategory;
        }

        //Printout for Debugging and to confirm what was sent (could be removed)
        System.out.println("This API was called:" + apiUrl);

        //Fetching Logic
        URL url = new URL(apiUrl);
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();
        conn.setRequestMethod("GET");

        InputStreamReader reader = new InputStreamReader(conn.getInputStream());

        //Safe retrieved Questions in the List
        Type listType = new TypeToken<List<TriviaQuestion>>() {}.getType();
        List<TriviaQuestion> questions = new Gson().fromJson(reader, listType);

        reader.close();
        conn.disconnect();

        return questions;
    }
}

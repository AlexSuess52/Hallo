package com.example.backendjava.controller;

import com.example.backendjava.model.Category;
import com.example.backendjava.model.Quiz;
import com.example.backendjava.repository.PlayerRepository;
import com.example.backendjava.repository.QuizRepository;
import com.example.backendjava.service.AiClientService;
import com.example.backendjava.service.TriviaApiClientService;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.*;

import java.util.*;

@RestController
@RequestMapping("/api")
public class MainQuizController {

    @Autowired
    private TriviaApiClientService triviaService;

    @Autowired
    private PlayerRepository playerRepository;

    @Autowired
    private QuizRepository quizRepository;

    @Autowired
    private AiClientService aiService;

    @Value("Online Quiz")
    private String mainTitle;

    @GetMapping("/quizzes/main")
    //Filter DB quizzes as otherwise dropdown explodes
    public List<Quiz> getFilteredQuizzes() {
        List<Quiz> allQuizzes = quizRepository.findAll();

        Map<String, Quiz> uniqueQuizzes = new LinkedHashMap<>();
        for (Quiz quiz : allQuizzes) {
            uniqueQuizzes.putIfAbsent(quiz.getQuizName(), quiz);
        }

        List<Quiz> filteredList = new ArrayList<>(uniqueQuizzes.values());
        filteredList.removeIf(quiz -> "AI Quiz".equals(quiz.getQuizName()));

        return filteredList; // JSON response
    }

    //API that will generate both Quizzes, AI and API
    @PostMapping("/player/generateQuiz")
    public Map<String, Object> login(@RequestParam("playerName") String name,
                                     @RequestParam("quizId") Long quizId,
                                     @RequestParam(value = "difficulty", required = false) String difficulty,
                                     @RequestParam String method) throws JsonProcessingException {
        String categoryName = null;

        // Try to fetch category name from the selected quiz (if present)
        Optional<Quiz> selectedQuizOpt = quizRepository.findById(quizId);
        if (selectedQuizOpt.isPresent()) {
            Quiz selectedQuiz = selectedQuizOpt.get();
            Category category = selectedQuiz.getCategory();
            categoryName = (category != null) ? category.getName() : null;
        }

        //Depending on Selection we need to make a different request
        if ("API".equalsIgnoreCase(method)) {
            Quiz apiQuiz = triviaService.generateAndSaveApiQuiz(difficulty, categoryName);
            quizId = apiQuiz.getId();
        } else if ("AI".equalsIgnoreCase(method)) {
            Quiz aiQuiz = aiService.generateAndSaveAiQuiz(difficulty, categoryName);
            quizId = aiQuiz.getId();
        }

        Map<String, Object> response = new HashMap<>();
        response.put("quizId", quizId);

        // print to check what is returned to frontend
        System.out.println("Login Response JSON: " + new ObjectMapper().writeValueAsString(response));

        return response;
    }
}


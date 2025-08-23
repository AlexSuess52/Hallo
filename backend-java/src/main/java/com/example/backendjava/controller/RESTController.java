package com.example.backendjava.controller;

import com.example.backendjava.model.*;
import com.example.backendjava.repository.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api")
public class RESTController {

    @Autowired
    private PlayerRepository playerRepository;

    @Autowired
    private CategoryRepository categoryRepository;

    @Autowired
    private QuizRepository quizRepository;

    @Autowired
    private QuizAnswerRepository quizAnswerRepository;

    @Autowired
    private QuizQuestionRepository quizQuestionRepository;

    @Autowired
    private QuizSessionRepository quizSessionRepository;

    // Player
    @GetMapping("/players")
    public List<Player> getAllPlayers() {
        return playerRepository.findAll();
    }

    // Category
    @GetMapping("/categories")
    public List<Category> getAllCategories() {
        return categoryRepository.findAll();
    }

    // Quiz
    @GetMapping("/quizzes")
    public List<Quiz> getAllQuizzes() {
        return quizRepository.findAll();
    }

    // QuizAnswer
    @GetMapping("/quiz-answers")
    public List<QuizAnswer> getAllQuizAnswers() {
        return quizAnswerRepository.findAll();
    }

    // QuizQuestion
    @GetMapping("/quiz-questions")
    public List<QuizQuestion> getAllQuizQuestions() {
        return quizQuestionRepository.findAll();
    }

    // QuizSession
    @GetMapping("/quiz-sessions")
    public List<QuizSession> getAllQuizSessions() {
        return quizSessionRepository.findAll();
    }
}

package com.example.backendjava.controller;

import com.example.backendjava.model.TriviaQuestion;
import com.example.backendjava.service.TriviaApiClientService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;


import java.util.List;

@RestController
@RequestMapping("/api/trivia")
public class TriviaController {

    @Autowired
    private TriviaApiClientService triviaApiClientService;

    @GetMapping("/questions")
    public List<TriviaQuestion> getTriviaQuestions() throws Exception {
        return triviaApiClientService.fetchRandomQuestions();
    }
}

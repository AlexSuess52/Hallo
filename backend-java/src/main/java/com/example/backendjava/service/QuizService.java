package com.example.backendjava.service;

import com.example.backendjava.model.*;
import com.example.backendjava.repository.PlayerRepository;
import com.example.backendjava.repository.QuizRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

@Service
public class QuizService {

    @Autowired
    private QuizRepository quizRepository;

    @Autowired
    private PlayerRepository playerRepository;

    @Autowired
    private QuizSessionService quizSessionService;

    // check if quiz id is valid and start a new quiz session
    public void startNewQuiz(String sessionId, Long quizId, Player player) {
        Quiz quiz = quizRepository.findById(quizId)
                .orElseThrow(() -> new IllegalArgumentException("Quiz not found with ID: " + quizId));
        quizSessionService.createSession(sessionId, player, quiz);
    }

    // find player in db by id
    public Player findPlayer(long playerId) {
        return playerRepository.findById(playerId).orElse(null);
    }
}

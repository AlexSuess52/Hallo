package com.example.backendjava.controller;

import com.example.backendjava.dto.*;
import com.example.backendjava.model.Player;
import com.example.backendjava.model.QuizQuestion;
import com.example.backendjava.service.QuizService;
import com.example.backendjava.service.QuizSessionService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.messaging.handler.annotation.Header;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.simp.annotation.SendToUser;
import org.springframework.stereotype.Controller;


@Controller
public class WebSocketController {

    @Autowired
    private QuizService quizService;

    @Autowired
    private QuizSessionService quizSessionService;

    @MessageMapping("/start")
    @SendToUser("/queue/next")
    public QuestionMessage handleStartQuiz(StartMessage message, @Header("simpSessionId") String sessionId) {
        // get and check if player exist
        long playerId = message.getPlayerId();
        Player player = quizService.findPlayer(playerId);
        if(player == null || message.getQuizId() == 0) {
            return null;
        }
        // start new quiz session
        quizService.startNewQuiz(sessionId, message.getQuizId(), player);
        // return first question
        return sendNextQuestion(sessionId);
    }

    @MessageMapping("/answer")
    @SendToUser("/queue/check")
    public CheckMessage handleAnswer(AnswerMessage message, @Header("simpSessionId") String sessionId) {
        // create check message and send back
        String correctAnswer = quizSessionService.getActualQuestion(sessionId).getCorrectAnswer();
        boolean isCorrect = quizSessionService.checkAnswer(sessionId, message.getAnswer());
        return new CheckMessage(correctAnswer, isCorrect);
    }

    @MessageMapping("/score")
    @SendToUser("/queue/score")
    public ScoreMessage handleScore(@Header("simpSessionId") String sessionId) {
        // check if session id exist
        if(quizSessionService.getSession(sessionId) != null) {
            // create score message
            ScoreMessage msg = new ScoreMessage(
                    quizSessionService.getPlayerName(sessionId),
                    quizSessionService.getQuizName(sessionId),
                    quizSessionService.getTotalScore(sessionId),
                    quizSessionService.getNrOfQuestions(sessionId),
                    quizSessionService.getTotalTime(sessionId));
            // finish session
            quizSessionService.finishSession(sessionId);
            return msg;
        } else {
            return null;
        }
    }

    @MessageMapping("/next")
    @SendToUser("/queue/next")
    public QuestionMessage nextQuestion(@Header("simpSessionId") String sessionId) {
        return sendNextQuestion(sessionId);
    }

    // create new question message to send
    private QuestionMessage sendNextQuestion(String sessionId) {
        String quizName = quizSessionService.getQuizName(sessionId);
        QuizQuestion nextQuestion = quizSessionService.getNextQuestion(sessionId);
        return new QuestionMessage(quizName, nextQuestion);
    }


}

package com.example.backendjava.service;

import com.example.backendjava.model.*;
import com.example.backendjava.repository.SessionsRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.*;

@Service
public class QuizSessionService {

    private final Map<String, QuizSession> quizSessions = new HashMap<>();

    @Autowired
    private SessionsRepository sessionsRepository;

    // create a new quiz session, add to session ap and set start time
    public void createSession(String sessionId, Player player, Quiz quiz) {
        quizSessions.remove(sessionId); // remove old session by id if exist
        QuizSession session = new QuizSession(player, quiz);
        quizSessions.put(sessionId, session);
        session.setStartTime(System.currentTimeMillis());
        saveSession(sessionId);
    }

    // get the next quiz question based on the session (return null if finished)
    public QuizQuestion getNextQuestion(String sessionId) {
        QuizSession session = quizSessions.get(sessionId);
        Quiz quiz = session.getQuiz();
        int currentQuestionIndex = session.getQuestionIndex();
        List<QuizQuestion> questions = quiz.getQuestions();
        if (currentQuestionIndex >= questions.size()) {
            return null; // No more questions => quiz finished
        }
        return quiz.getQuestions().get(session.getQuestionIndex());
    }

    // get the actual quiz question of the given session
    public QuizQuestion getActualQuestion(String sessionId) {
        QuizSession session = quizSessions.get(sessionId);
        Quiz quiz = session.getQuiz();
        return quiz.getQuestions().get(session.getQuestionIndex());
    }

    // check if the user answer is correct (based on the question entry)
    public boolean checkAnswer(String sessionId, String userAnswer) {
        boolean correct = false;
        // get the actual quiz question
        QuizSession session = quizSessions.get(sessionId);
        QuizQuestion q = getActualQuestion(sessionId);
        // save answer to the session entry
        session.addQuizAnswer(new QuizAnswer(q, userAnswer));
        // check if correct and increase score
        if (q != null && q.getCorrectAnswer().equals(userAnswer)) {
            correct = true;
            session.increaseScore(1);
        }
        // update session time
        session.setTotalTime(System.currentTimeMillis() - session.getStartTime());
        // increase the question index
        session.increaseQuestionIndex();
        return correct;
    }

    public int getNrOfQuestions(String sessionId) {
        return quizSessions.get(sessionId).getQuiz().getQuestions().size();
    }

    public QuizSession getSession(String sessionId) {
        return quizSessions.get(sessionId);
    }

    public int getTotalScore(String sessionId) {
        return quizSessions.get(sessionId).getTotalScore();
    }

    public long getTotalTime(String sessionId) {
        return quizSessions.get(sessionId).getTotalTime();
    }

    public String getQuizName(String sessionId) {
        return quizSessions.get(sessionId).getQuiz().getQuizName();
    }

    public String getPlayerName(String sessionId) {
        return quizSessions.get(sessionId).getPlayer().getName();
    }

    // save and remove the session
    public void finishSession(String sessionId) {
        saveSession(sessionId);
        removeSession(sessionId);
    }

    private void removeSession(String sessionId) {
        quizSessions.remove(sessionId);
    }

    private void saveSession(String sessionId) {
        sessionsRepository.save(quizSessions.get(sessionId));
    }
}

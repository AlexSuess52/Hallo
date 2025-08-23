package com.example.backendjava.service;

import com.example.backendjava.model.*;
import com.example.backendjava.repository.SessionsRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.test.context.ContextConfiguration;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;

@ContextConfiguration(classes= QuizSessionService.class)
@ExtendWith(MockitoExtension.class)
public class QuizSessionServiceTest {
    @InjectMocks
    private QuizSessionService quizSessionService;

    @Mock
    private SessionsRepository sessionsRepository;

    private Player player;
    private Quiz quiz;
    private List<QuizQuestion> questions;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);

        player = new Player();
        player.setName("TestPlayer");

        questions = Arrays.asList(
                new QuizQuestion("Capital of France?", List.of("Paris", "London", "Rome"), "Paris"),
                new QuizQuestion("5 + 3?", List.of("6", "7", "8"), "8")
        );

        quiz = new Quiz();
        quiz.setQuizName("General Knowledge");
        for (QuizQuestion question : questions) {
            quiz.addQuestion(question);
        }
    }

    @Test
    void testCreateSessionAndGetSession() {
        quizSessionService.createSession("session1", player, quiz);

        QuizSession session = quizSessionService.getSession("session1");
        assertNotNull(session);
        assertEquals("TestPlayer", session.getPlayer().getName());
        assertEquals("General Knowledge", session.getQuiz().getQuizName());

        verify(sessionsRepository, times(1)).save(session);
    }

    @Test
    void testGetNextQuestion() {
        quizSessionService.createSession("session1", player, quiz);

        QuizQuestion question = quizSessionService.getNextQuestion("session1");
        assertEquals("Capital of France?", question.getQuestionText());
    }

    @Test
    void testGetActualQuestion() {
        quizSessionService.createSession("session1", player, quiz);

        QuizQuestion question = quizSessionService.getActualQuestion("session1");
        assertEquals("Capital of France?", question.getQuestionText());
    }

    @Test
    void testCheckAnswer_Correct() {
        quizSessionService.createSession("session1", player, quiz);
        quizSessionService.getNextQuestion("session1"); // simulate step

        boolean isCorrect = quizSessionService.checkAnswer("session1", "Paris");
        QuizSession session = quizSessionService.getSession("session1");

        assertTrue(isCorrect);
        assertEquals(1, session.getTotalScore());
        assertEquals(1, session.getQuestionIndex());
    }

    @Test
    void testCheckAnswer_Incorrect() {
        quizSessionService.createSession("session1", player, quiz);
        quizSessionService.getNextQuestion("session1");

        boolean isCorrect = quizSessionService.checkAnswer("session1", "London");
        QuizSession session = quizSessionService.getSession("session1");

        assertFalse(isCorrect);
        assertEquals(0, session.getTotalScore());
        assertEquals(1, session.getQuestionIndex());
    }

    @Test
    void testGetNrOfQuestions() {
        quizSessionService.createSession("session1", player, quiz);
        int num = quizSessionService.getNrOfQuestions("session1");
        assertEquals(2, num);
    }

    @Test
    void testGetTotalScoreAndTime() {
        quizSessionService.createSession("session1", player, quiz);
        quizSessionService.getNextQuestion("session1");
        quizSessionService.checkAnswer("session1", "Paris");

        long totalTime = quizSessionService.getTotalTime("session1");
        int score = quizSessionService.getTotalScore("session1");

        assertTrue(totalTime >= 0);
        assertEquals(1, score);
    }

    @Test
    void testGetQuizAndPlayerName() {
        quizSessionService.createSession("session1", player, quiz);

        assertEquals("General Knowledge", quizSessionService.getQuizName("session1"));
        assertEquals("TestPlayer", quizSessionService.getPlayerName("session1"));
    }

    @Test
    void testFinishSessionRemovesSession() {
        quizSessionService.createSession("session1", player, quiz);
        assertNotNull(quizSessionService.getSession("session1"));

        quizSessionService.finishSession("session1");

        assertNull(quizSessionService.getSession("session1"));
        verify(sessionsRepository, times(2)).save(any()); // once on create, once on finish
    }
}

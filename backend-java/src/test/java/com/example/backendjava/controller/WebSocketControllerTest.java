package com.example.backendjava.controller;

import com.example.backendjava.dto.*;
import com.example.backendjava.model.Player;
import com.example.backendjava.model.QuizQuestion;
import com.example.backendjava.model.QuizSession;
import com.example.backendjava.service.QuizService;
import com.example.backendjava.service.QuizSessionService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.test.context.ContextConfiguration;

import java.util.List;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.*;

@ContextConfiguration(classes= WebSocketController.class)
@ExtendWith(MockitoExtension.class)
public class WebSocketControllerTest {
    @InjectMocks
    private WebSocketController webSocketController;

    @Mock
    private QuizService quizService;

    @Mock
    private QuizSessionService quizSessionService;

    private final String sessionId = "session123";
    private final Player mockPlayer = new Player();

    @BeforeEach
    void setup() {
        MockitoAnnotations.openMocks(this);
        mockPlayer.setName("Test Player");
    }

    @Test
    void testHandleStartQuiz_success() {
        StartMessage message = new StartMessage();
        message.setPlayerId(1L);
        message.setQuizId(2L);

        QuizQuestion mockQuestion = new QuizQuestion("Question?", List.of("A", "B"), "A");

        when(quizService.findPlayer(1L)).thenReturn(mockPlayer);
        doNothing().when(quizService).startNewQuiz(eq(sessionId), eq(2L), eq(mockPlayer));
        when(quizSessionService.getQuizName(sessionId)).thenReturn("QuizName");
        when(quizSessionService.getNextQuestion(sessionId)).thenReturn(mockQuestion);

        QuestionMessage result = webSocketController.handleStartQuiz(message, sessionId);

        assertNotNull(result);
        assertEquals("QuizName", result.getQuizName());
        assertEquals(mockQuestion, result.getQuizQuestion());
    }

    @Test
    void testHandleStartQuiz_invalidPlayer() {
        StartMessage message = new StartMessage();
        message.setPlayerId(1L);
        message.setQuizId(2L);

        when(quizService.findPlayer(1L)).thenReturn(null);

        QuestionMessage result = webSocketController.handleStartQuiz(message, sessionId);
        assertNull(result);
    }

    @Test
    void testHandleAnswer() {
        AnswerMessage answerMessage = new AnswerMessage();
        answerMessage.setAnswer("A");

        QuizQuestion q = new QuizQuestion("Q?", List.of("A", "B"), "A");

        when(quizSessionService.getActualQuestion(sessionId)).thenReturn(q);
        when(quizSessionService.checkAnswer(sessionId, "A")).thenReturn(true);

        CheckMessage result = webSocketController.handleAnswer(answerMessage, sessionId);

        assertEquals("A", result.getCorrectAnswer());
        assertTrue(result.isCorrect());
    }

    @Test
    void testHandleScore_existingSession() {
        when(quizSessionService.getSession(sessionId)).thenReturn(new QuizSession()); // mock non-null session
        when(quizSessionService.getPlayerName(sessionId)).thenReturn("Test Player");
        when(quizSessionService.getQuizName(sessionId)).thenReturn("Sample Quiz");
        when(quizSessionService.getTotalScore(sessionId)).thenReturn(2);
        when(quizSessionService.getNrOfQuestions(sessionId)).thenReturn(3);
        when(quizSessionService.getTotalTime(sessionId)).thenReturn(123L);

        ScoreMessage result = webSocketController.handleScore(sessionId);

        assertEquals("Test Player", result.getPlayerName());
        assertEquals("Sample Quiz", result.getQuizName());
        assertEquals(2, result.getScore());
        assertEquals(3, result.getCount());
        assertEquals(123L, result.getTime());

        verify(quizSessionService).finishSession(sessionId);
    }

    @Test
    void testHandleScore_noSession() {
        when(quizSessionService.getSession(sessionId)).thenReturn(null);

        ScoreMessage result = webSocketController.handleScore(sessionId);
        assertNull(result);
    }

    @Test
    void testNextQuestion() {
        QuizQuestion q = new QuizQuestion("Next Q?", List.of("1", "2"), "1");

        when(quizSessionService.getQuizName(sessionId)).thenReturn("Test Quiz");
        when(quizSessionService.getNextQuestion(sessionId)).thenReturn(q);

        QuestionMessage result = webSocketController.nextQuestion(sessionId);

        assertEquals("Test Quiz", result.getQuizName());
        assertEquals(q, result.getQuizQuestion());
    }
}

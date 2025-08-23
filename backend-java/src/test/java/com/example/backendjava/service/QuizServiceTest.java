package com.example.backendjava.service;

import com.example.backendjava.model.Player;
import com.example.backendjava.model.Quiz;
import com.example.backendjava.repository.PlayerRepository;
import com.example.backendjava.repository.QuizRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.test.context.ContextConfiguration;

import java.util.Optional;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ContextConfiguration(classes= QuizService.class)
@ExtendWith(MockitoExtension.class)
public class QuizServiceTest {
    @InjectMocks
    private QuizService quizService;

    @Mock
    private QuizRepository quizRepository;

    @Mock
    private PlayerRepository playerRepository;

    @Mock
    private QuizSessionService quizSessionService;

    private Player player;
    private Quiz quiz;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);

        player = new Player();
        player.setId(1L);
        player.setName("John");

        quiz = new Quiz();
        quiz.setId(100L);
        quiz.setQuizName("Java Basics");
    }

    @Test
    void testStartNewQuiz_Valid() {
        when(quizRepository.findById(100L)).thenReturn(Optional.of(quiz));

        quizService.startNewQuiz("session123", 100L, player);

        verify(quizRepository).findById(100L);
        verify(quizSessionService).createSession("session123", player, quiz);
    }

    @Test
    void testStartNewQuiz_QuizNotFound() {
        when(quizRepository.findById(999L)).thenReturn(Optional.empty());

        Exception ex = assertThrows(IllegalArgumentException.class, () ->
                quizService.startNewQuiz("sessionX", 999L, player)
        );

        assertEquals("Quiz not found with ID: 999", ex.getMessage());
        verify(quizRepository).findById(999L);
        verifyNoInteractions(quizSessionService);
    }

    @Test
    void testFindPlayerById_Found() {
        when(playerRepository.findById(1L)).thenReturn(Optional.of(player));

        Player result = quizService.findPlayer(1L);
        assertNotNull(result);
        assertEquals("John", result.getName());
    }

    @Test
    void testFindPlayerById_NotFound() {
        when(playerRepository.findById(42L)).thenReturn(Optional.empty());

        Player result = quizService.findPlayer(42L);
        assertNull(result);
    }
}

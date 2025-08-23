package com.example.backendjava.service;

import com.example.backendjava.model.Category;
import com.example.backendjava.model.Quiz;
import com.example.backendjava.repository.CategoryRepository;
import com.example.backendjava.repository.QuizRepository;
import com.example.backendjava.model.TriviaQuestion;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.*;
import java.util.*;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

class TriviaApiClientServiceTest {

    @InjectMocks
    private TriviaApiClientService triviaApiClientService;

    @Mock
    private QuizRepository quizRepository;

    @Mock
    private CategoryRepository categoryRepository;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void generateAndSaveApiQuiz_shouldCreateQuizWithCategoryAndQuestions() throws Exception {
        // Arrange
        String difficulty = "easy";
        String categoryName = "Science";

        TriviaQuestion q1 = new TriviaQuestion("What is 2+2?", "4", Arrays.asList("2", "3", "5"));
        TriviaQuestion q2 = new TriviaQuestion("What planet?", "Mars", Arrays.asList("Venus", "Jupiter", "Saturn"));

        List<TriviaQuestion> mockQuestions = Arrays.asList(q1, q2);

        // Mock fetchQuestions manually (so we don't call real API)
        TriviaApiClientService spyClient = Mockito.spy(triviaApiClientService);
        doReturn(mockQuestions).when(spyClient).fetchQuestions(difficulty, categoryName);

        Category mockCategory = new Category();
        mockCategory.setName("Science");
        when(categoryRepository.findByNameIgnoreCase("Science")).thenReturn(Optional.of(mockCategory));

        Quiz savedQuiz = new Quiz("API Quiz", "mock");
        when(quizRepository.save(any(Quiz.class))).thenReturn(savedQuiz);

        // Act
        Quiz quiz = spyClient.generateAndSaveApiQuiz(difficulty, categoryName);

        // Assert
        assertNotNull(quiz);
        verify(quizRepository, times(1)).save(any(Quiz.class));
        verify(categoryRepository, times(1)).findByNameIgnoreCase("Science");

        // Optional: assert quiz content
        ArgumentCaptor<Quiz> quizCaptor = ArgumentCaptor.forClass(Quiz.class);
        verify(quizRepository).save(quizCaptor.capture());
        Quiz capturedQuiz = quizCaptor.getValue();
        assertEquals(2, capturedQuiz.getQuestions().size());
        assertEquals("API Quiz", capturedQuiz.getQuizName());
    }
}

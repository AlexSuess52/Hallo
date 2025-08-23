package com.example.backendjava.service;

import com.example.backendjava.model.Category;
import com.example.backendjava.model.Quiz;
import com.example.backendjava.model.QuizQuestion;
import com.example.backendjava.repository.CategoryRepository;
import com.example.backendjava.repository.QuizRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.*;
import org.springframework.test.util.ReflectionTestUtils;

import java.util.*;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

class AiClientServiceTest {

    @InjectMocks
    private AiClientService aiClientService;

    @Mock
    private QuizRepository quizRepository;

    @Mock
    private CategoryRepository categoryRepository;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
        // Set dummy API key so RestTemplate isn't actually called
        ReflectionTestUtils.setField(aiClientService, "openAiApiKey", "test-key");
    }

    @Test
    void generateAndSaveAiQuiz_shouldCreateQuizWithParsedQuestionsAndCategory() throws Exception {
        // Prepare mock AI response
        String aiMockText = """
            Question: What is the capital of France?
            A) Berlin
            B) Madrid
            C) Rome
            D) Paris
            Answer: D

            Question: What color is the sky?
            A) Green
            B) Red
            C) Blue
            D) Yellow
            Answer: C
        """;

        // Spy the service to mock the private method parseAiResponseToQuestions()
        AiClientService spyService = spy(aiClientService);
        List<QuizQuestion> mockQuestions = List.of(
                new QuizQuestion("What is the capital of France?", List.of("Berlin", "Madrid", "Rome", "Paris"), "Paris"),
                new QuizQuestion("What color is the sky?", List.of("Green", "Red", "Blue", "Yellow"), "Blue")
        );
        doReturn(mockQuestions).when(spyService).parseAiResponseToQuestions(anyString());

        // Also mock fetchAiQuestions (to avoid real HTTP call)
        doReturn(mockQuestions).when(spyService).fetchAiQuestions("easy", "Geography");

        // Mock category
        Category mockCategory = new Category();
        mockCategory.setName("Geography");
        when(categoryRepository.findByNameIgnoreCase("Geography")).thenReturn(Optional.of(mockCategory));

        // Mock save
        when(quizRepository.save(any(Quiz.class))).thenAnswer(invocation -> invocation.getArgument(0));

        // Act
        Quiz quiz = spyService.generateAndSaveAiQuiz("easy", "Geography");

        // Assert
        assertNotNull(quiz);
        assertEquals("AI Quiz", quiz.getQuizName());
        assertEquals(2, quiz.getQuestions().size());
        assertEquals("Geography", quiz.getCategory().getName());

        verify(quizRepository, times(1)).save(any(Quiz.class));
    }
}

package com.example.backendjava.service;

import com.example.backendjava.model.Category;
import com.example.backendjava.model.Quiz;
import com.example.backendjava.model.QuizQuestion;
import com.example.backendjava.repository.CategoryRepository;
import com.example.backendjava.repository.QuizRepository;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.*;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

import java.util.*;

@Service
public class AiClientService {

    private final String aiUrl = "https://api.openai.com/v1/chat/completions";

    @Value("${openai.api.key}")
    private String openAiApiKey;

    @Autowired
    private QuizRepository quizRepository;

    @Autowired
    private CategoryRepository categoryRepository;

    //This Method reads the fetched Qs from OpenAI and stores them in DB for the Controllers to read later on (similar logic to API Fetching)
    public Quiz generateAndSaveAiQuiz(String difficulty, String categoryName) {
        try {
            List<QuizQuestion> aiQuestions = fetchAiQuestions(difficulty, categoryName);

            Quiz quiz = new Quiz("AI Quiz", "This quiz was generated using OpenAI");

            // Assign category
            if (categoryName != null && !categoryName.isBlank()) {
                Optional<Category> categoryOpt = categoryRepository.findByNameIgnoreCase(categoryName.trim());
                categoryOpt.ifPresent(quiz::setCategory);
            }

            for (QuizQuestion question : aiQuestions) {
                quiz.addQuestion(question);
            }

            return quizRepository.save(quiz);

        } catch (Exception e) {
            throw new RuntimeException("Failed to fetch or save AI quiz: " + e.getMessage(), e);
        }
    }

    //Making the actual request to OpenAI
    protected List<QuizQuestion> fetchAiQuestions(String difficulty, String categoryName) throws Exception {
        RestTemplate restTemplate = new RestTemplate();

        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_JSON);
        headers.setBearerAuth(openAiApiKey);

        //Define strict format, otherwise AI will vary in it's response
        String prompt = String.format("""
        Generate 5 multiple-choice quiz questions in the category '%s' with difficulty '%s'. 
        For each question, use the following format exactly:
        
        Question: <question text>
        A) <option 1>
        B) <option 2>
        C) <option 3>
        D) <option 4>
        Answer: <A/B/C/D>
        
        Separate each question with a blank line.
        """, categoryName, difficulty);

        // Print the prompt for debugging
        System.out.println("Prompt sent to OpenAI:");
        System.out.println(prompt);

        Map<String, Object> body = new HashMap<>();
        body.put("model", "gpt-3.5-turbo");
        body.put("messages", List.of(Map.of("role", "user", "content", prompt)));
        body.put("temperature", 0.7);

        HttpEntity<Map<String, Object>> request = new HttpEntity<>(body, headers);
        ResponseEntity<String> response = restTemplate.postForEntity(aiUrl, request, String.class);

        ObjectMapper mapper = new ObjectMapper();
        JsonNode root = mapper.readTree(response.getBody());
        String aiText = root.path("choices").get(0).path("message").path("content").asText();

        // Print the response from OpenAI
        System.out.println("Response from OpenAI:");
        System.out.println(aiText);

        return parseAiResponseToQuestions(aiText);
    }

    //Parsing the AI Response from text to JSON
    protected List<QuizQuestion> parseAiResponseToQuestions(String aiText) {
        List<QuizQuestion> questions = new ArrayList<>();

        // This assumes AI response is structured clearly (should be as we provide strict format in request).
        String[] rawQuestions = aiText.split("\\n\\n");
        for (String raw : rawQuestions) {
            String[] lines = raw.split("\\n");
            if (lines.length < 5) continue;

            String question = lines[0].replaceFirst("^Question:\\s*", "").trim();
            String a = lines[1].replaceFirst("^A\\)\\s*", "").trim();
            String b = lines[2].replaceFirst("^B\\)\\s*", "").trim();
            String c = lines[3].replaceFirst("^C\\)\\s*", "").trim();
            String d = lines[4].replaceFirst("^D\\)\\s*", "").trim();
            String answerLetter = lines[5].replaceFirst("^Answer:\\s*", "").trim().toUpperCase();

            List<String> answers = List.of(a, b, c, d);
            String correct = switch (answerLetter) {
                case "A" -> a;
                case "B" -> b;
                case "C" -> c;
                case "D" -> d;
                default -> a; // fallback
            };
            questions.add(new QuizQuestion(question, answers, correct));
        }
        return questions;
    }
}

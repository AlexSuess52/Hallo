package com.example.backendjava;

import com.example.backendjava.model.Category;
import com.example.backendjava.model.Quiz;
import com.example.backendjava.model.QuizQuestion;
import com.example.backendjava.repository.CategoryRepository;
import com.example.backendjava.repository.QuizRepository;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Bean;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Optional;
import java.util.stream.Stream;

import java.util.List;

@SpringBootApplication
public class BackendJavaApplication {

	public static void main(String[] args) {
		SpringApplication.run(BackendJavaApplication.class, args);
	}

	@Bean
	CommandLineRunner initDatabase(QuizRepository quizRepository, CategoryRepository categoryRepository) {
		System.out.println("Initializing database...");

		return args -> {

			//  Read categories from I/O file and save to DB
			try (InputStream inputStream = getClass().getClassLoader()
					.getResourceAsStream("static/apiCategories.txt")) {

				if (inputStream != null) {
					List<String> lines = new BufferedReader(new InputStreamReader(inputStream))
							.lines()
							.limit(10)
							.toList();

					for (String name : lines) {
						String trimmedName = name.trim();

						//Checks if Entry already exists so it avoids program crash on startup (duplicates in Category are no bueno for Java)
						boolean exists = categoryRepository.findByNameIgnoreCase(trimmedName).isPresent();
						if (exists) {
							continue;
						}
						Category c = new Category();
						c.setName(trimmedName);
						Category saved = categoryRepository.save(c);
						//Confirmation on save
						System.out.println("Saved category: ID=" + saved.getId() + ", name='" + saved.getName() + "'");
					}
				} else {
					System.err.println("File not found: static/apiCategories.txt");
				}

			} catch (IOException e) {
				System.err.println("Failed to load category file: " + e.getMessage());
			}

			//  Read quizzes from file and save
			try (InputStream inputStream = getClass().getClassLoader()
					.getResourceAsStream("static/apiQuiz.txt")) {

				if (inputStream != null) {
					List<String> lines = new BufferedReader(new InputStreamReader(inputStream))
							.lines()
							.limit(10)
							.toList();

					for (String line : lines) {
						String[] parts = line.split(",", 2); // split at first comma
						if (parts.length < 2) continue;

						String quizName = parts[0].trim();
						String description = parts[1].trim();

						//Extract normalized category name from quiz name
						String rawCategory = quizName.replace(" Quiz", "").trim().toLowerCase();

						//Replace _ with space
						String normalizedCategory = rawCategory.replace(" ", "_");

						// Find matching category so it can be linked up
						Optional<Category> categoryOpt = categoryRepository.findByNameIgnoreCase(normalizedCategory);
						if (categoryOpt.isPresent()) {
							Category category = categoryOpt.get();
							Quiz quiz = new Quiz(quizName, description);
							quiz.setCategory(category); // Assigns found Cat to Quiz
							quizRepository.save(quiz); //  Save to DB
						} else {
							System.err.println("Category not found for quiz: " + quizName);
						}

					}
				} else {
					System.err.println("File not found: static/apiQuiz.txt");
				}

			} catch (IOException e) {
				System.err.println("Failed to load quiz file: " + e.getMessage());
			}
		};
	}
}

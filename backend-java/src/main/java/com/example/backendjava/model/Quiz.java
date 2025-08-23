package com.example.backendjava.model;


import com.fasterxml.jackson.annotation.JsonManagedReference;
import jakarta.persistence.*;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Entity
public class Quiz {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String quizName;
    private String description;

    private LocalDateTime createdAt = LocalDateTime.now();

    @ManyToOne
    @JoinColumn(name = "category_id")
    private Category category;

    @OneToMany(mappedBy = "quiz", cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.EAGER)
    @JsonManagedReference
    private List<QuizQuestion> questions = new ArrayList<>();


    public Quiz() {}

    public Quiz(String quizName) {
        this.quizName = quizName;
    }

    public Quiz(String quizName, String description) {
        this.quizName = quizName;
        this.description = description;
    }

    public Quiz(String quizName, String description, Category category) {
        this.quizName = quizName;
        this.description = description;
        this.category = category;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public Category getCategory() {
        return category;
    }

    public void setCategory(Category category) {
        this.category = category;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public String getQuizName() {
        return quizName;
    }

    public void setQuizName(String quizName) {
        this.quizName = quizName;
    }

    public List<QuizQuestion> getQuestions() {
        return questions;
    }

    public void addQuestion(QuizQuestion question) {
        question.setQuiz(this);
        this.questions.add(question);
    }
}

package com.example.backendjava.model;

import jakarta.persistence.*;

import java.util.Objects;

@Entity
public class QuizAnswer {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "question_id")
    private QuizQuestion question;

    private String answer;
    private boolean correct;

    @ManyToOne
    @JoinColumn(name = "session_id")
    private QuizSession session;

    public QuizAnswer() {}

    public QuizAnswer(QuizQuestion question, String answer) {
        this.question = question;
        this.answer = answer;
        this.correct = Objects.equals(this.answer, this.question.getCorrectAnswer());
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public QuizQuestion getQuestion() {
        return question;
    }

    public void setQuestion(QuizQuestion question) {
        this.question = question;
    }

    public String getAnswer() {
        return answer;
    }

    public void setAnswer(String answer) {
        this.answer = answer;
    }

    public boolean isCorrect() {
        return correct;
    }

    public void setCorrect(boolean correct) {
        this.correct = correct;
    }

    public QuizSession getSession() {
        return session;
    }

    public void setSession(QuizSession session) {
        this.session = session;
    }
}

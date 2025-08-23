package com.example.backendjava.dto;

import com.example.backendjava.model.QuizQuestion;

public class QuestionMessage {

    private String quizName;
    private QuizQuestion quizQuestion;

    public QuestionMessage() {}

    public QuestionMessage(String quizName, QuizQuestion quizQuestion) {
        this.quizName = quizName;
        this.quizQuestion = quizQuestion;
    }

    public String getQuizName() {
        return quizName;
    }

    public void setQuizName(String quizName) {
        this.quizName = quizName;
    }

    public QuizQuestion getQuizQuestion() {
        return quizQuestion;
    }

    public void setQuizQuestion(QuizQuestion quizQuestion) {
        this.quizQuestion = quizQuestion;
    }
}

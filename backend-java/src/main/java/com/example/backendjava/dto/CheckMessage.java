package com.example.backendjava.dto;

public class CheckMessage {

    private String correctAnswer;
    private boolean isCorrect;

    public CheckMessage() {}

    public CheckMessage(String correctAnswer, boolean isCorrect) {
        this.correctAnswer = correctAnswer;
        this.isCorrect = isCorrect;
    }

    public String getCorrectAnswer() {
        return correctAnswer;
    }

    public void setCorrectAnswer(String correctAnswer) {
        this.correctAnswer = correctAnswer;
    }

    public boolean isCorrect() {
        return isCorrect;
    }

    public void setCorrect(boolean isCorrect) {
        this.isCorrect = isCorrect;
    }
}

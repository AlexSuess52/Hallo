package com.example.backendjava.model;

import jakarta.persistence.*;

import java.util.*;

@Entity
public class QuizSession {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "player_id")
    private Player player;

    @ManyToOne
    @JoinColumn(name = "quiz_id")
    private Quiz quiz;

    @OneToMany(mappedBy = "session", cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.EAGER)
    private List<QuizAnswer> quizAnswers = new ArrayList<>();

    private long startTime;
    private long totalTime;
    private int totalScore;
    private int questionIndex;


    public QuizSession() {}

    public QuizSession(Player player, Quiz quiz) {
        this.player = player;
        this.quiz = quiz;
        this.totalScore = 0;
        this.totalTime = 0;
        this.questionIndex = 0;
    }

    public void setId(Long id) {
        this.id = id;
    }

    public Long getId() {
        return id;
    }

    public Player getPlayer() {
        return player;
    }

    public void setPlayer(Player player) {
        this.player = player;
    }

    public Quiz getQuiz() {
        return quiz;
    }

    public void setQuiz(Quiz quiz) {
        this.quiz = quiz;
    }

    public long getStartTime() {
        return startTime;
    }

    public void setStartTime(long startTime) {
        this.startTime = startTime;
    }

    public long getTotalTime() {
        return totalTime;
    }

    public void setTotalTime(long totalTime) {
        this.totalTime = totalTime;
    }

    public void addQuizAnswer(QuizAnswer answer) {
        answer.setSession(this);
        quizAnswers.add(answer);
    }

    public void increaseScore(int points) {
        this.totalScore += points;
    }

    public int getTotalScore() {
        return totalScore;
    }

    public int getQuestionIndex() {
        return questionIndex;
    }

    public void setQuestionIndex(int questionIndex) {
        this.questionIndex = questionIndex;
    }

    public void increaseQuestionIndex() {
        this.questionIndex++;
    }
}

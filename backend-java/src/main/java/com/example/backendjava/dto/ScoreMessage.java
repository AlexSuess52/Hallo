package com.example.backendjava.dto;

public class ScoreMessage {

    private String playerName;
    private String quizName;
    private int score;
    private int count;
    private long time;

    public ScoreMessage(){}

    public ScoreMessage(String playerName, String quizName, int score, int count, long time) {
        this.playerName = playerName;
        this.quizName = quizName;
        this.score = score;
        this.count = count;
        this.time = time;
    }

    public String getPlayerName() {
        return playerName;
    }

    public void setPlayerName(String playerName) {
        this.playerName = playerName;
    }

    public String getQuizName() {
        return quizName;
    }

    public void setQuizName(String quizName) {
        this.quizName = quizName;
    }

    public int getScore() {
        return score;
    }

    public void setScore(int score) {
        this.score = score;
    }

    public int getCount() {
        return count;
    }

    public void setCount(int count) {
        this.count = count;
    }

    public long getTime() {
        return time;
    }

    public void setTime(long time) {
        this.time = time;
    }
}

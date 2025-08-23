package com.example.backendjava.dto;


public class StartMessage {
    private long playerId;
    private long quizId;

    public StartMessage() {}

    public StartMessage(long playerId, long quizId) {
        this.playerId = playerId;
        this.quizId = quizId;
    }

    public long getQuizId() {
        return quizId;
    }

    public void setQuizId(long quizId) {
        this.quizId = quizId;
    }

    public long getPlayerId() {
        return playerId;
    }

    public void setPlayerId(long playerId) {
        this.playerId = playerId;
    }
}

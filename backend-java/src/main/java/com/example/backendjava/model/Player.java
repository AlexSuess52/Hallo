package com.example.backendjava.model;


import jakarta.persistence.*;

import java.time.LocalDateTime;

@Entity
@Table(name = "players", schema = "players")
@org.hibernate.annotations.Immutable
public class Player {
    @Id
    private Long id;

    @Column(name = "name")
    private String name;

    @Column(name = "password_hash")
    private String passwordHash;

    @Column(name = "refresh_token")
    private String refreshToken;

    @Column(name = "refresh_token_expiry_time")
    private LocalDateTime refreshTokenExpiryTime;

    @Column(name = "user_email")
    private String userEmail;

    @Column(name = "created_at")
    private LocalDateTime createdAt;

    public Player() {}

    public Player(String name) {
        this.name = name;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
        this.id = id;
    }
}

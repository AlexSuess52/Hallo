package com.example.backendjava.repository;

import com.example.backendjava.model.QuizSession;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface SessionsRepository extends JpaRepository<QuizSession, Long> {}

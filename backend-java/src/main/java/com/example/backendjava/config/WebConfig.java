package com.example.backendjava.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.*;

//We must allow this for cross-origin scripting as we run the apps on different ports
@Configuration
public class WebConfig implements WebMvcConfigurer {

    @Override
    public void addCorsMappings(CorsRegistry registry) {
        registry.addMapping("/**") // apply to all endpoints
                .allowedOrigins("*")
                .allowedMethods("*")
                .allowedHeaders("*");
    }
}

package icd0011.config;

import org.springframework.context.annotation.*;
import org.springframework.web.servlet.config.annotation.EnableWebMvc;

@Import(SpringConfig.class)
@EnableWebMvc

public class MvcConfig {


}
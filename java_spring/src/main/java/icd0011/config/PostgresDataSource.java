package icd0011.config;

import org.springframework.context.annotation.Bean;
import org.springframework.core.env.Environment;
import org.springframework.core.io.ClassPathResource;
import org.springframework.jdbc.datasource.DriverManagerDataSource;
import org.springframework.jdbc.datasource.init.DatabasePopulatorUtils;
import org.springframework.jdbc.datasource.init.ResourceDatabasePopulator;

import javax.sql.DataSource;

//@Configuration
public class PostgresDataSource {

    private Environment env;

    public PostgresDataSource(Environment env) {
        this.env = env;
    }

    @Bean
    public DataSource dataSource() {
        DriverManagerDataSource ds = new DriverManagerDataSource();
        ds.setDriverClassName("org.postgresql.Driver");
        ds.setUsername(env.getProperty("postgres.user"));
        ds.setPassword(env.getProperty("postgres.pass"));
        ds.setUrl(env.getProperty("postgres.url"));

        var populator = new ResourceDatabasePopulator(
                new ClassPathResource("schema.sql"));

        DatabasePopulatorUtils.execute(populator, ds);

        return ds;
    }

}
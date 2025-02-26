package icd0011.order;

import icd0011.config.HsqlDataSource;
import icd0011.config.SpringConfig;
import icd0011.model.Order;
import icd0011.model.OrderRow;
import org.springframework.context.ConfigurableApplicationContext;
import org.springframework.context.annotation.AnnotationConfigApplicationContext;
import java.util.List;

public class Main {

    public static void main(String[] args){
        ConfigurableApplicationContext ctx =
                new AnnotationConfigApplicationContext(
                        SpringConfig.class, HsqlDataSource.class);

        try (ctx) {

           // OrderDao dao = ctx.getBean(OrderDao.class);

        }

        }
    }


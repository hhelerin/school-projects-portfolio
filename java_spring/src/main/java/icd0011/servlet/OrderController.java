package icd0011.servlet;

import icd0011.model.Order;
import icd0011.order.OrderDao;
import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.sql.SQLException;
import java.util.List;

@RestController
public class OrderController {

    private OrderDao dao;

    public OrderController(OrderDao dao) {
        this.dao = dao;
    }

    @GetMapping("orders")
    public List getPosts() throws SQLException {

        return dao.getOrders();
    }

    @GetMapping("orders/{id}")
    public Order getPostById(@PathVariable Long id) throws SQLException {

        return dao.getOrderById(id);
    }

    @ResponseStatus(HttpStatus.CREATED)
    @PostMapping("orders")
    public Order createPost(@RequestBody @Valid Order order) {

        return dao.insertOrder(order);
    }

    @DeleteMapping("orders/{id}")
    public void deletePost(@PathVariable Long id) {

        dao.deleteOrderById(id);
    }
}
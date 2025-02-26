/*package icd0011.order;

import icd0011.model.Order;
import icd0011.model.OrderRow;
import org.springframework.jdbc.core.RowCallbackHandler;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.LinkedList;
import java.util.List;

public class OrderRowHandler implements RowCallbackHandler {
    *//* default *//* List<Order> result = new LinkedList<>();

    @Override
    public void processRow(ResultSet rs) throws SQLException {
        Long orderId = rs.getLong("id");
        String orderNumber = rs.getString("orderNumber");
        String itemName = rs.getString("itemName");
        Integer quantity = rs.getInt("quantity");
        Integer price = rs.getInt("price");

        OrderRow row = new OrderRow(itemName, quantity, price);
        Order order = contains(result, orderId);

        if(order == null){
            order = new Order(orderId, orderNumber, null);
            order.addOrderRow(row);
            result.add(order);
        }else {
            order.addOrderRow(row);
        }
    }

    public List<Order> getResult() {
        return result;
    }

    private Order contains(List<Order> orderList, Long id) {
        for (Order order : orderList) {
            if (order.getId().equals(id)) {
                return order;
            }
        }
        return null;
    }
}*/

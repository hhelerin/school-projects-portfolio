package icd0011.order;

import icd0011.model.Order;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import jakarta.persistence.TypedQuery;
import jakarta.transaction.Transactional;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;
import java.sql.*;
import java.util.List;



@Repository
public class OrderDao {
    @PersistenceContext
    private EntityManager em;

    public List getOrders() {
        return em.createQuery("select o from Order o").getResultList();
    }

        public Order getOrderById(Long id){

           TypedQuery<Order> query = em.createQuery("select o from Order o where o.id = :id",
                   Order.class);

            query.setParameter("id", id);

            return query.getSingleResult();
        }

    @Transactional
    public Order insertOrder(Order order) {
            em.persist(order);
        return order;
    }


    @Transactional
    public void deleteOrderById(Long id) {

        Order order = em.find(Order.class, id);

        if(order != null){
            em.remove(order);
        }

    }

}


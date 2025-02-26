package icd0011.model;

import jakarta.persistence.*;
import jakarta.validation.Valid;
import jakarta.validation.constraints.Size;
import lombok.*;

import java.util.ArrayList;
import java.util.List;

@Builder
@AllArgsConstructor
@NoArgsConstructor
@RequiredArgsConstructor
@Data
@Entity
@Table(name = "orders")

public class Order {
    @Id
    @SequenceGenerator(name = "my_seq", sequenceName = "seq1", allocationSize = 1)
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "my_seq")
    private Long id;

    @Size(min = 2)
    @Column(name = "order_number")
    @NonNull
    private String orderNumber;

    @Valid
    @ElementCollection(fetch = FetchType.EAGER)
    @NonNull
    @CollectionTable(
            name = "order_rows",
            joinColumns=@JoinColumn(name = "orders_id",
                    referencedColumnName = "id"))
    private List<OrderRow> orderRows;

    public void addOrderRow(OrderRow row) {
        if (orderRows == null) {
            orderRows = new ArrayList<>();
        }
        orderRows.add(row);
    }

}


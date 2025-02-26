package icd0011.model;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import jakarta.persistence.*;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
@Embeddable
@Table(name = "order_rows")
@JsonIgnoreProperties(ignoreUnknown = true)

public class OrderRow {
    @Column(name = "item_name")
    private String itemName;

    @NotNull
    @Min(1)
    @Column(name = "quantity")
    private Integer quantity;

    @NotNull
    @Min(1)
    @Column(name = "price")
    private Integer price;
}

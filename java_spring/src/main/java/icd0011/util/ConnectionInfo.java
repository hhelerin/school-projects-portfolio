package icd0011.util;

import lombok.AllArgsConstructor;
import lombok.Value;

@Value
@AllArgsConstructor
public class ConnectionInfo {

    private final String url;
    private final String user;
    private final String pass;

}

import React, { Component } from 'react';
import {Grid, Row, Col} from 'react-bootstrap';

class LoginContainer extends Component {
    render() {
        return (
            <Grid>
                <Row className="show-grid">
                    <Col xs={12} md={8}>
                        TEst
                    </Col>
                    <Col xs={6} md={4}>
                        Test
                    </Col>
                </Row>
            </Grid>
        );
    }
}

export default LoginContainer;
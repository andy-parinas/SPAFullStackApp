import React, {Component} from 'react';
import axios from 'axios';


class ValuePage extends Component{

    constructor(props){
        super(props);
        this.state ={
            values: []
        }
    }

    componentDidMount(){
        axios.get("http://localhost:5000/api/values").then(res => {
            this.setState({
                ...this.state,
                values: res.data
            })
        }).catch(err => {
            console.log(err);
        })
    }

    render(){

        const values = this.state.values.map(value =>{
            return <li>{value.name}</li>
        })

        return(
            <div>
                <h1>Value Page</h1>
                <h2>Data</h2>
                <ul>
                    {values}
                </ul>
            </div>
        )
    }


}

export default ValuePage;
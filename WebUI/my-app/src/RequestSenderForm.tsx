import {useCallback, useState} from "react";
import {Box, Button, TextField, Typography} from "@mui/material";

const Form = () => {
    
    const [url, setUrl] = useState<string>("http://localhost:5072/api/Rate"); 
    const [numberOfRequests, setNumberOfRequests] = useState<number>(1)
    
    const sendRequests = useCallback(() => {
        for(let i = 0; i < numberOfRequests; i++){
            fetch(url).then((response) => response.json())
                .then((data) => console.log(data));
        }
    }, [url, numberOfRequests])
    
    return <>
        <Box alignSelf={'center'} component="form" sx={{ 
        '& .MuiTextField-root': { m: 1, width: '25ch' },
    }}>
            <TextField required={true} type="url" id="outlined-basic" label={"url"} onChange={value => setUrl(value.target.value)} value={url}/>
            <TextField required={true} type="number" id="" label="# of requests" value={numberOfRequests} onChange={value => setNumberOfRequests(Number.parseInt(value.target.value))} />
            <Button variant="contained" form={"form"} type={"submit"} onClick={sendRequests}>Send</Button>
        </Box>
    </>
}


export default Form;
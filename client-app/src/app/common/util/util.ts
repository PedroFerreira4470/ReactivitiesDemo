export  const combineDateAndTime = (date:Date, time:Date)=>{
    // const timeString = time.getHours() + ':' + time.getMinutes() + ':00';

    // const year= date.getFullYear() ;
    // const month=date.getMonth() + 1;
    // const day =date.getDay();

    // const dateString = `${year}-${month}-${day}`;
    const dateString = date.toISOString().split('T')[0];
    const timeString = time.toISOString().split('T')[1];

    return new Date(dateString +'T'+ timeString);
}